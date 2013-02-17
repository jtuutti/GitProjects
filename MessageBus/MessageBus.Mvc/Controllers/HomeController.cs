using System;
using System.Threading.Tasks;
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
        public async Task<ViewResult> Index(string textField)
        {
            int id;

            if (!Int32.TryParse(textField, out id))
            {
                ViewBag.ResponseText = new MvcHtmlString(String.Format("Bus returned: <b>{0}</b>", MessageTypeEnum.Unknown));
                return View();
            }

            try
            {
                bus.Events.FaultOccurred += (sender, args) =>
                {
                    ViewBag.ResponseText = new MvcHtmlString(String.Format("Bus faulted: <pre>{0}</pre>", args.FaultException));
                };

                var response = await bus.SendAndReceive<MessageTypeEnum>(new Command { Id = id });

                if (ViewBag.ResponseText == null)
                {
                    ViewBag.ResponseText = new MvcHtmlString(String.Format("Bus returned: <b>{0}</b>", response));
                }
            }
            catch(Exception ex)
            {
                ViewBag.ResponseText = new MvcHtmlString(String.Format("Bus faulted: <pre>{0}</pre>", ex));
            }

            return View();
        }
    }
}
