using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace UF2Practica1
{
	class MainClass
	{
		//Valors constants
		#region Constants
	
        const int tCaixes = 10;//per si es canvia al text, mes rapid
       
		#endregion
		/* Cua concurrent
		 	Dos mètodes bàsics: 
		 		Cua.Enqueue per afegir a la cua
		 		bool success = Cua.TryDequeue(out clientActual) per extreure de la cua i posar a clientActual
		*/

		public static ConcurrentQueue<Client> cua = new ConcurrentQueue<Client>();

		public static void Main(string[] args)
		{
            int i;
            int caixeres;
           
            Console.WriteLine("**********************");
            Console.WriteLine("*     APU MARKET     *");
            Console.WriteLine("**********************");

            //primera accio, demanar caixeres
            Console.WriteLine("Introdueix el numero de caixeres.");
            Console.WriteLine("Hi ha un total de " + tCaixes + " caixes.");
            Caixera c = new UF2Practica1.Caixera() ;//instancio un objecte de la clase caixera per accedir al metode que demana les caixeres.
            caixeres = c.triaCaixeres();
           

            var clock = new Stopwatch();
			var threads = new List<Thread>();

			//Recordeu-vos que el fitxer CSV ha d'estar a la carpeta bin/debug de la solució
			const string fitxer = "CuaClients.csv";

			try
			{
				var reader = new StreamReader(File.OpenRead(@fitxer));


				//Carreguem la llista clients

				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(';');
					var tmp = new Client() { nom = values[0], carretCompra = Int32.Parse(values[1]) };
					cua.Enqueue(tmp);//Enqueue equeueeja clients a la cua

				}

			}
			catch (Exception)
			{
				Console.WriteLine("Error accedint a l'arxiu");
				Console.ReadKey();
				Environment.Exit(0);
			}

			clock.Start();


            // Instanciar les caixeres i afegir el thread creat a la llista de threads
            for (i = 0; i < caixeres; i++)
            {
                var cTemp = new UF2Practica1.Caixera() { idCaixera = i };
                var Thread = new Thread(() => cTemp.ProcessarCua());
                Thread.Start();
                threads.Add(Thread);
            }

			// Procediment per esperar que acabin tots els threads abans d'acabar
			foreach (Thread thread in threads)
				thread.Join();//bloqueja fil fins que acaba la cua???/******* aclarir amb en Carlos *******/
            
			// Parem el rellotge i mostrem el temps que triga
			clock.Stop();
			double temps = clock.ElapsedMilliseconds / 1000;
			Console.Clear();
			Console.WriteLine("Temps total Task: " + temps + " segons");
			Console.ReadKey();
		}
	}
   
	#region ClassCaixera
	public class Caixera
	{

        public int idCaixera
		{
			get;
			set;
		}

		public void ProcessarCua()
		{
            // Llegirem la cua extreient l'element
            // cridem al mètode ProcesarCompra passant-li el client

            while (!MainClass.cua.IsEmpty) {//mentre no estigu buida  
                var temp = new Client();
                MainClass.cua.TryDequeue(out temp);// TryDequeue desqueueja de la cua
                ProcesarCompra(temp);
            }


		}
        
		private void ProcesarCompra(Client client)
		{

			Console.WriteLine("La caixera " + this.idCaixera + " comença amb el client " + client.nom + " que té " + client.carretCompra + " productes");

			for (int i = 0; i < client.carretCompra; i++)
			{
				this.ProcessaProducte();

			}

			Console.WriteLine(">>>>>> La caixera " + this.idCaixera + " ha acabat amb el client " + client.nom);
		}


		private void ProcessaProducte()
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));//retrasa 1 segon
		}

        public int triaCaixeres()//demana caixeres
        {
          
            int tCaixeres = 0;
           

            String totalCaixeres = Console.ReadLine();
            bool isInt = Int32.TryParse(totalCaixeres, out tCaixeres);//conversió a integer SEMPRE OUT try parse
            if (!isInt) 
                Console.WriteLine("Entra un valor correcte");
        
                comprovaResposta(tCaixeres);
            return tCaixeres;

        }


        public void comprovaResposta( int tCaixeres) {
            if (tCaixeres < 1 || tCaixeres > 10)
            {
                Console.WriteLine("Entra un valor correcte entre 1 i 10");
                triaCaixeres();
            }
            else
                Console.WriteLine("Has donat d'alta " + tCaixeres + " caixeres");
            
        }
    }


	#endregion

	#region ClassClient

	public class Client
	{
		public string nom
		{
			get;
			set;
		}


		public int carretCompra
		{
			get;
			set;
		}


	}

	#endregion
}
