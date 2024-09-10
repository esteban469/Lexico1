using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

/*
    Requerimiento 1: Sobrecargar el constructor Lexico para que reciba como
                     argumento el nombre del archvo a compilar
    Requerimiento 2: Tener un contador de lineas
    Requerimiento 3: Agregar OperadorRelacional:
                     ==,>,>=,<,<=,<>,!=           
    Requerimiento 4: Agregar OperadorLogico
                     &&,||,!
    Requerimiento 5: Antes de cerrar los archivos, contar cuantas lineas contamos (todo esto en el dispose)                 
*/
namespace Lexico1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int lineCount = 0;



        public Lexico()
        {
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;

            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
            contarLineas();
        }
        //***************************REQUERIMIENTO 1***********************

        public Lexico(string nuevoArchivo)
        {


            if (File.Exists(nuevoArchivo) && Path.GetExtension(nuevoArchivo) == ".cpp")
            {

                log = new StreamWriter("nuevoArchivo.log");
                asm = new StreamWriter("nuevoArchivo.asm");
                log.AutoFlush = true;
                asm.AutoFlush = true;
                archivo = new StreamReader(nuevoArchivo);
            }
            else
            {
                throw new Error("El archivo no es un archivo.cpp o no existe", log);
            }

        }


        //**********************FIN DE REQUERIMIENTO 1********************************


        public void Dispose()
        {
            log.WriteLine($"El archivo Lexico.cs tiene: {lineCount} lineas");
            archivo.Close();
            log.Close();
            asm.Close();

        }
        //********* CONTADOR DE LINEAS ***************
        public void contarLineas()
        {
            using (StreamReader leerDoc = new StreamReader("Lexico.cs"))
            {
                string line;
                while ((line = leerDoc.ReadLine()) != null)
                {
                    lineCount++;
                }
            }
        }
        //********* FIN CONTADOR DE LINEAS ***************
        public void nextToken()
        {
            char c;
            string buffer = "";

            while (char.IsWhiteSpace(c = (char)archivo.Read()))
            {

            }

            buffer += c;

            if (char.IsLetter(c))
            {
                setClasificacion(Tipos.Identificador);
                while (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == ';')
            {
                setClasificacion(Tipos.FinSentencia);
            }
            else if (c == '{')
            {
                setClasificacion(Tipos.InicioBloque);
            }
            else if (c == '}')
            {
                setClasificacion(Tipos.FinBloque);
            }
            else if (c == '?')
            {
                setClasificacion(Tipos.OperadorTernario);
            }
            else if (c == '=')
            {
                setClasificacion(Tipos.Asignacion);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '+')
            {
                setClasificacion(Tipos.OperadorTermino);
                if ((c = (char)archivo.Peek()) == '+' || c == '=')
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '-')
            {
                setClasificacion(Tipos.OperadorTermino);
                if ((c = (char)archivo.Peek()) == '-' || (c == '='))
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                }
                else if (c == '>')
                {
                    setClasificacion(Tipos.Puntero);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '*' || c == '/' || c == '%')
            {
                setClasificacion(Tipos.OperadorFactor);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.IncrementoFactor);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '>')
            {
                setClasificacion(Tipos.OperadorRelacional);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }/*Agregar OperadorRelacional:
                     <,<=,<>,!=) */
            else if (c == '<')
            {
                setClasificacion(Tipos.OperadorRelacional);
                if ((c = (char)archivo.Peek()) == '=' || c == '>')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }//Agregar OperadorLogico
             //  (&&,||,!)
            else if (c == '!')
            {
                setClasificacion(Tipos.OperadorLogico);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '&')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '&')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '|')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '|')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (c == '$')
            {
                setClasificacion(Tipos.Caracter);
                if (char.IsDigit(c = (char)archivo.Peek()))
                {
                    setClasificacion(Tipos.moneda);
                    buffer += c;
                    archivo.Read();
                    while (char.IsDigit(c = (char)archivo.Peek()))
                    {
                        buffer += c;
                        archivo.Read();
                    }
                }
            }


            else
            {
                setClasificacion(Tipos.Caracter);
            }

            if (!finArchivo())
            {

                setContenido(buffer);
                log.WriteLine(getContenido() + " ------ " + getClasificacion());
            }


        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}