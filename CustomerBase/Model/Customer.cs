using CustomerBase.Framework;

namespace CustomerBase.Model
{
    // Representa um cliente.
    public class Customer : Bindable
    {
        private int? id;
        public int? Id
        {
            get { return id; }
            set
            {
                SetValue(ref id, value);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                SetValue(ref name, value);

                if (string.IsNullOrEmpty(name))
                {
                    AddError("Name cannot be empty.");
                }
                else
                {
                    RemoveErrors();
                }
            }
        }

        private string? email;
        public string? Email
        {
            get { return email; }
            set
            {
                SetValue(ref email, value);
            }
        }

        private string? phoneNumber;
        public string? PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                SetValue(ref phoneNumber, value);
            }
        }

        private Address address;
        public Address Address
        {
            get
            {
                if (address == null)
                {
                    address = new Address();
                }

                return address;
            }
            set
            {
                SetValue(ref address, value);
            }
        }
    }
}
