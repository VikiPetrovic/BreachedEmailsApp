using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Runtime;
using BreachedEmailsApp.Helpers;

namespace BreachedEmailsApp
{
    public interface IDomainGrain : IGrainWithStringKey
    {
        /* Grain interface for communication with the grain across the cluster. 
         * Inherits from IGrainWithStringKey - because string identity
         * */
        Task Create(Email email);
        Task<string> Get(Email email);
    }

    public class DomainGrain : Grain, IDomainGrain
    {
        private IPersistentState<EmailsState> _emails;      //TState (EmailsState) mora bit seriazable type
        private IDisposable _timer;     // timer for persistence
        
        public DomainGrain([PersistentState("emails", "emailsStore")] IPersistentState<EmailsState> emails)
        {
            this._emails = emails;  // persistent state se doda grainu z injectanjem preko kontruktorja (recommended way)
        }

        public Task<HashSet<string>> GetEmailsAsync() => Task.FromResult(_emails.State.BreachedEmails);        // gets all emails in state

        public async Task Create(Email email)    // adds email to state and persists state to storage
        {
            //inserts new email into state hashset - if exists, it also returns OK            
            _emails.State.BreachedEmails.Add(email.address);
           // await _emails.WriteStateAsync();

            //throw new NotImplementedException();
        }

        public async Task<string> Get(Email email)
        {
            if (_emails.State.BreachedEmails.Contains(email.address)){
                // return email
                return email.address;
            }
            else
            {
                return null;
            }           
            
            //throw new NotImplementedException();
        }

        
        public override Task OnActivateAsync()
        {
            //register timer on grain activation
            _timer = RegisterTimer(async func =>
            {
                //function to execute on timer
                await _emails.WriteStateAsync();
            },
            null,        //state that we want to be passed to the function
            TimeSpan.FromSeconds(0),     // initial delay from first invocation
            TimeSpan.FromMinutes(5)     // interval on which the function above is executed
            );

            return base.OnActivateAsync();
        }
    }
}
