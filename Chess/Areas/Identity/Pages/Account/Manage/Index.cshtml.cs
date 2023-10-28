// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Utils;

namespace Chess.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment webHostEnvironment,
			IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Avatar")]
            public IFormFile AvatarFile { get; set; }
            public string Avatar { get; set; }
			[Required]
			[MinLength(5)]
            public string Username { get; set; }

        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Email = await _userManager.GetEmailAsync(user);
            
            Input = new InputModel
            {
                Username = userName,
                Avatar = user.AvatarPath
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if(Input.Username != user.UserName)
            {
                user.UserName = Input.Username;
                user.NormalizedUserName = Input.Username.ToUpper();

            }
            
            if (Input.AvatarFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.AvatarFile.FileName);

                string imageFolderPath = Path.Combine(wwwRootPath, SD.AvatarsPath);
                string imagePath = Path.Combine(imageFolderPath, fileName);


                if (!string.IsNullOrEmpty(user.AvatarPath) && user.AvatarPath != SD.StandartIcon)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, user.AvatarPath.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await Input.AvatarFile.CopyToAsync(fileStream);
                }

                user.AvatarPath = @"\" + SD.AvatarsPath + @"\" + fileName;
            }
            
            await _userManager.UpdateAsync(user);
            _unitOfWork.User.Update(user);
            _unitOfWork.Save();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
