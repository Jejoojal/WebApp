using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.ReCaptcha;
using Newtonsoft.Json;

namespace WebApp.Pages.Profiles
{
    [ValidateReCaptcha]
    public class Profile
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Etunimi")]
        [RegularExpression(@"[A-ö]+$")]
        public string etunimi { get; set; }

        [Required]
        [Display(Name = "Sukunimi")]
        [RegularExpression(@"[A-ö]+$")]
        public string sukunimi { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Sähköposti")]
        public string sposti { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Puhelinnumero")]
        public string puhelin { get; set; }
        [Required]
        [Display(Name = "Katuosoite")]
        [RegularExpression(@"([A-ö0-9 ])+")]
        public string katuosoite { get; set; }
        [Required]
        [Display(Name = "Postinumero")]
        [RegularExpression(@"[0-9]+")]
        public string postinumero { get; set; }
        [Required]
        [Display(Name = "Postitoimipaikka")]
        [RegularExpression(@"[A-ö]+")]
        public string postitoimipaikka { get; set; }

        public string JSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
