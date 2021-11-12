using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Data;
using AspNetCore.ReCaptcha;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Diagnostics;

namespace WebApp.Pages.Profiles
{
    [ValidateReCaptcha]
    public class CreateModel : PageModel
    {
        private readonly WebApp.Data.WebAppContext _context;

        public CreateModel(WebApp.Data.WebAppContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Profile Profile { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string json = Profile.JSON();

            string queueName = Profile.puhelin + "lomake";

            //Pilvessä tässä pitäisi käyttää Azuren connectionstringiä
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            QueueClient queue = new QueueClient(connectionString, queueName);

            if (null != await queue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("The queue was created.");
            }

            await queue.SendMessageAsync(json);

            return Page();
        }
    }
}
