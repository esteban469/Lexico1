using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico1
{
    class Program
    {
        static void Main(string[] args)
        {
           string archivoCPP = @"prueba.cpp"; 
            try
            {
                using (Lexico l = new Lexico())
                {
                    while (!l.finArchivo())
                    {
                        l.nextToken();
                        
                    }
                }
             
            int contadorLinea = File.ReadLines(archivoCPP).Count();
            Console.WriteLine($"El archivo {archivoCPP} tiene {contadorLinea} líneas.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            
        }
    }
}
