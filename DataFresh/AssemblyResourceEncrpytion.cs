using System;

namespace DataFresh
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyResourceEncrpytion : Attribute
    {
        private bool enryptionEnabled;
        private string enryptionKey = "nopassword";

        public AssemblyResourceEncrpytion()
        {
            enryptionEnabled = true;
        }
		
        public AssemblyResourceEncrpytion(bool enabled)
        {
            enryptionEnabled = enabled;
        }

        public virtual bool Enabled 
        {
            get 
            {
                return enryptionEnabled;
            }
            set
            {
                enryptionEnabled = value;
            }
        }
		
        public virtual string Key 
        {
            get 
            {
                return enryptionKey;
            }		
        }
    }
}