using System;
using System.Web.Mvc;
using MessageBus.Mvc.Messages;

namespace MessageBus.Mvc.Controllers
{
    public class HomeController : AsyncController
    {
        private readonly IBus bus;

        public HomeController(IBus bus)
        {
            if (bus == null) throw new ArgumentNullException("bus");
            this.bus = bus;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        [AsyncTimeout(30000)]
        public void IndexAsync(string textField)
        {
            int number;
            if (!Int32.TryParse(textField, out number)) return;

            AsyncManager.OutstandingOperations.Increment();

            var command = new Command { Id = number };

            bus.SendAsync(command, r =>
                                   {
                                       try
                                       {
                                           AsyncManager.Parameters["response"] = bus.SendComplete<MessageTypeEnum>(r);
                                       }
                                       catch (Exception ex)
                                       {
                                           AsyncManager.Parameters["ex"] = ex;
                                       }
									   finally
									   {
                                           AsyncManager.OutstandingOperations.Decrement();
									   }
                                   });
        }

        public ActionResult IndexCompleted(MessageTypeEnum response, Exception ex)
        {
            if (ex != null)
            {
                ViewBag.ResponseText = new MvcHtmlString(String.Format("Bus faulted: <pre>{0}</pre>", ex));
            }
            else
            {
                ViewBag.ResponseText = new MvcHtmlString(String.Format("Bus returned: <b>{0}</b>", response));              
            }

            return View("Index");
        }
    }
}
