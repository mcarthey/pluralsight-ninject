using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;

namespace Ninject.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup to bind dependencies
            var kernel = new StandardKernel();
            kernel.Bind<ICreditCard>().To<MasterCard>().InSingletonScope();        // don't need to register Shopper - automatic

            var shopper = kernel.Get<Shopper>();
            shopper.Charge();
            Console.WriteLine(shopper.ChargesForCurrentCard);

            // separate instances typically - can convert to Singleton (Bind.InSingletonScope())
            var shopper2 = kernel.Get<Shopper>();
            shopper2.Charge();
            Console.WriteLine(shopper2.ChargesForCurrentCard);

            // can also bind to a method
            kernel.Rebind<ICreditCard>().ToMethod(context =>
            {
                Console.WriteLine("Creating new card!");
                return new Visa();
            });
            var shopper3 = kernel.Get<Shopper>();
            shopper3.Charge();
            Console.WriteLine(shopper3.ChargesForCurrentCard);

            // example to move all bindings to module
            var kernel2 = new StandardKernel(new MyModule());
            var shopper4 = kernel2.Get<Shopper>();
            shopper4.Charge();
            Console.WriteLine(shopper4.ChargesForCurrentCard);
        }
    }

    public class MyModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ICreditCard>().To<MasterCard>();
        }
    }

    public class Shopper
    {
        private readonly ICreditCard _creditCard;

        public Shopper(ICreditCard creditCard)
        {
            _creditCard = creditCard;
        }

        public int ChargesForCurrentCard
        {
            get { return _creditCard.ChargeCount; }
        }

        public void Charge()
        {
            Console.WriteLine(_creditCard.Charge());
        }
    }

    public class Visa : ICreditCard
    {
        public string Charge()
        {
            return "Using the Visa!";
        }

        public int ChargeCount
        {
            get { return 0; }
        }
    }
    public class MasterCard : ICreditCard
    {
        public string Charge()
        {
            ChargeCount++;
            return "Charging with the MasterCard!";
        }

        public int ChargeCount { get; set; }
    }

    public interface ICreditCard
    {
        string Charge();
        int ChargeCount { get; }
    }
}
