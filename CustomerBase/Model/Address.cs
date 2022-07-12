using CustomerBase.Framework;

namespace CustomerBase.Model
{
    // Representa um endereço.
    public class Address : Bindable
    {
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set
            {
                SetValue(ref customer, value);
            }
        }

        private string street;
        public string Street
        {
            get { return street; }
            set
            {
                SetValue(ref street, value);

                if (string.IsNullOrEmpty(street))
                {
                    AddError("Street cannot be empty.");
                }
                else
                {
                    RemoveErrors();
                }
            }
        }

        private int? number;
        public int? Number
        {
            get { return number; }
            set
            {
                SetValue(ref number, value);

                if (number == null)
                {
                    AddError("Number cannot be empty.");
                }
                else
                {
                    RemoveErrors();
                }
            }
        }

        private string? complement;
        public string? Complement
        {
            get { return complement; }
            set
            {
                SetValue(ref complement, value);
            }
        }

        private string? district;
        public string? District
        {
            get { return district; }
            set
            {
                SetValue(ref district, value);
            }
        }

        private string zipCode;
        public string ZipCode
        {
            get { return zipCode; }
            set
            {
                SetValue(ref zipCode, value);

                if (string.IsNullOrEmpty(zipCode))
                {
                    AddError("Zip code cannot be empty.");
                }
                else
                {
                    RemoveErrors();
                }
            }
        }
    }
}
