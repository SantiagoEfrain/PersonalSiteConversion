using Microsoft.AspNetCore.Mvc;
using PersonalSiteConversion.Models;
using System.Diagnostics;
using MimeKit;
using MailKit.Net.Smtp;

namespace PersonalSiteConversion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Models()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            //Check if the model is valid before you do anything else.
            if (!ModelState.IsValid)
            {
                //Send them back to the form. We can pass the object to the View
                //So the form will contain the original information they provided.
                return View(cvm);
            }

            //To handle sending the email, we'll need to install a NuGet package and 
            //add a few using statements.

            #region Email Setup Steps & Email Info

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for NETCore.MailKit
            //3. Click NETCore.MailKit
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here
            //6. Add the following using statements & comments:
            //      - using MimeKit; //Added for access to MimeMessage class
            //      - using MailKit.Net.Smtp; //Added for access to SmtpClient class
            //7. Once added, return here to continue coding email functionality
            #endregion

            //MIME - Multipurpose Internet Mail Extensions - Allows email to contain
            //information other than ASCII, including audio, video, images, and HTML
            //S/MIME -> Secure Apps / PKI encryption

            //SMTP - Simple Mail Transfer Protocol - An internet Protocol, similar to HTTP, that
            //specializes in the collection & processing of email data.

            //Create the format for the message content we will receive from the contact form.
            string message = $"You have received a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name}<br/>" +
                $"Email: {cvm.Email}<br/>" +
                $"Subject: {cvm.Subject}<br/>" +
                $"Message: {cvm.Message}";

            //Create a MimeMessage object to assist with storing/transfering the email information
            //from the contact form.
            var mm = new MimeMessage();

            //Even though the user is the one attempting to send a message to us, the actual sender of the email
            //is the "email user" we set up in SmarterASP

            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));
            //The recipient of this email will be our personal email address, also stored in appsettings.json
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //We can add the user's provided email address to the list of ReplyTo adresses so our replies
            //can be sent directly to them instead of the "email user" on smarter asp.
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The subject will be the subject provided by the user in the form. We have this stored in the cvm object.
            mm.Subject = cvm.Subject;

            //if we don't have any HTML, we can assign the body as mm.Body = message.
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the message so it will be flagged in our email client.
            mm.Priority = MessagePriority.Urgent;

            //The using directive will create an SMTP Client object used to send the email.
            //Once all of the code inside is done, it will close any open connections and dispose the object for us.
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_config.GetValue<string>("Credentials:Email:Client"));
                    //log in to the mail server using the credentials for our email user
                    client.Authenticate(
                        //username
                        _config.GetValue<string>("Credentials:Email:User"),
                        //password
                        _config.GetValue<string>("Credentials:Email:Password")
                        );
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there is an issue, we can store an error message in a ViewBag variable to be displayed in the View
                    ViewBag.ErrorMessage = $"There was an issue processing your request. Please" +
                        $" try again later.<br/>Error Message: {ex.Message}<br>" +
                        $"{_config.GetValue<string>("Credentials:Email:Client")}";
                    return View(cvm);
                }
            }//end using

            //if all goes well, return a View that displays a confirmation to the user that their email was sent
            //successfully
            return View("EmailConfirmation", cvm);
        }

    }
}