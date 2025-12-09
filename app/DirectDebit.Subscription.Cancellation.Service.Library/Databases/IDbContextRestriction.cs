using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Databases
{
	public interface IDbContextRestriction
	{
		//Gets the username of the user who is modifying DB sets 
		public string GetUserName();
	}
}
