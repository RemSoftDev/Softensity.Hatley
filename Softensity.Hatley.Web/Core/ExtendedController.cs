using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public abstract class BaseController : Controller
    {
		protected RedirectToRouteResult RedirectToAction<T>(Expression<Action<T>> action) where T : Controller
		{
			var body = action.Body as MethodCallExpression;

			if (body == null)
			{
				throw new ArgumentException("Expression must be a method call.");
			}

			if (body.Object != action.Parameters[0])
			{
				throw new ArgumentException("Method call must target lambda argument.");
			}

			string actionName = body.Method.Name;

			var attributes = body.Method.GetCustomAttributes(typeof(ActionNameAttribute), false);
			if (attributes.Length > 0)
			{
				var actionNameAttr = (ActionNameAttribute)attributes[0];
				actionName = actionNameAttr.Name;
			}

			string controllerName = typeof(T).Name;

			if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
			{
				controllerName = controllerName.Remove(controllerName.Length - 10, 10);
			}

			return RedirectToAction(actionName, controllerName);
		}
    }
}
