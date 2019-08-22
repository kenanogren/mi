using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace mi.Models.ViewModel
{
    public class register
    {
        [Required(ErrorMessage ="Boş bırakılamaz")]
        [Display(Name ="İsim")]
        public string ad { get; set; }

        [Required(ErrorMessage = "Boş bırakılamaz")]
        [Display(Name = "Soyisim")]
        public string soyad { get; set; }

        [Required(ErrorMessage = "Boş bırakılamaz")]
        [Display(Name = "Eposta")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Geçerli bir email adresi giriniz")]
        public string email { get; set; }

        [Required(ErrorMessage = "Boş bırakılamaz")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string sifre { get; set; }

        [Required(ErrorMessage = "Boş bırakılamaz")]
        [Display(Name = "Tekrar Şifre")]
        [DataType(DataType.Password)]
        [Compare("sifre",ErrorMessage ="şifre öncekiyle eşleşmiyor")]
        public string Resifre { get; set; }
        public string password { get; internal set; }
        public string comfirmPassword { get; internal set; }
    }
}