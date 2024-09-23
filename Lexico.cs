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
        int contLineaPruebaCPP = 0;



        public Lexico()
        {
            //lineCount = 1;
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

            contarLineasPruebaCPP();

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
                throw new Error("El archivo no es un .cpp o no existe", log);
            }
            contarLineas();

        }


        //**********************FIN DE REQUERIMIENTO 1********************************


        public void Dispose()
        {

            archivo.Close();
            log.Close();
            asm.Close();

        }
        //********* CONTADOR DE LINEAS ***************
        public void contarLineas()
        {
            using (StreamReader leerDoc = new StreamReader("nuevoArchivo.cpp"))
            {
                string line;
                while ((line = leerDoc.ReadLine()) != null)
                {
                    lineCount++;
                }
            }
            log.WriteLine($"El archivo nuevoArchivo.cpp tiene: {lineCount} lineas");
        }


        public void contarLineasPruebaCPP()
        {
            using (StreamReader leerPrueba = new StreamReader("prueba.cpp"))
            {
                string line;
                while ((line = leerPrueba.ReadLine()) != null)
                {
                    contLineaPruebaCPP++;
                }
            }
            log.WriteLine($"El archivo prueba.cpp tiene: {contLineaPruebaCPP} lineas");
        }

        //********* FIN CONTADOR DE LINEAS ***************

        public void nextToken()
        {
            char c;
            string buffer = "";

            while (char.IsWhiteSpace(c = (char)archivo.Read()))
            {
                if (c == '\n')
                {
                    lineCount++;
                }
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
            //********************* OPERADOR NUMERO INICIO *********************

            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
                if (c == '.')
                {
                    buffer += c;
                    archivo.Read();
                    if (char.IsDigit(c = (char)archivo.Peek()) || c == 'e' || c == 'E')//
                    { //                   
                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            buffer += c;
                            archivo.Read();
                        }
                        if ((c = (char)archivo.Peek()) == 'e' || (c = (char)archivo.Peek()) == 'E')
                        {
                            buffer += c;
                            archivo.Read();
                            if (char.IsDigit(c = (char)archivo.Peek()) || c == '+' || c == '-') //
                            {
                                while (char.IsDigit(c = (char)archivo.Peek()))
                                {
                                    buffer += c;
                                    archivo.Read();
                                }
                                if ((c = (char)archivo.Peek()) == '+' || c == '-')
                                {
                                    buffer += c;
                                    archivo.Read();
                                    if (char.IsDigit(c = (char)archivo.Peek()))
                                    {
                                        while (char.IsDigit(c = (char)archivo.Peek()))
                                        {
                                            buffer += c;
                                            archivo.Read();
                                        }
                                    }
                                    else
                                    {
                                        throw new Error($"ERROR LEXICO: Se esperaba un digito despues de (+,-) en la linea {lineCount + 1}", log);
                                    }
                                }
                            }
                            else
                            {
                                throw new Error($"ERROR LEXICO: Se esperaba un digito despues de (e,E) en la linea {lineCount + 1}", log);
                            }

                        }
                    }
                    else
                    {
                        throw new Error($"ERROR LEXICO: Se esperaba un digito despues del punto en la linea {lineCount + 1}", log);
                    }
                }


                else if ((c = (char)archivo.Peek()) == 'e' || c == 'E')
                {
                    buffer += c;
                    archivo.Read();
                    if (char.IsDigit(c = (char)archivo.Peek()) || c == '+' || c == '-')
                    {
                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            buffer += c;
                            archivo.Read();
                        }
                        if ((c = (char)archivo.Peek()) == '+' || c == '-')
                        {
                            buffer += c;
                            archivo.Read();
                            if (char.IsDigit(c = (char)archivo.Peek()))
                            {
                                while (char.IsDigit(c = (char)archivo.Peek()))
                                {
                                    buffer += c;
                                    archivo.Read();
                                }
                            }
                            else
                            {
                                throw new Error($"ERROR LEXICO: Se esperaba un digito despues de (+,-) en la linea {lineCount + 1}", log);
                            }
                        }
                    }
                    else
                    {
                        throw new Error($"ERROR LEXICO: Se esperaba un digito en la linea {lineCount + 1}", log);
                    }
                }

            }


            //********************* OPERADOR NUMERO FIN **************************
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
            //***************** OPERADOR CADENA INICIO *************************
            else if (c == '"')
            {
                setClasificacion(Tipos.Caracter);
                c = (char)archivo.Read();

                setClasificacion(Tipos.Cadena);
                buffer += c;

                while (c != '"')
                {
                    if (finArchivo())
                    {
                        throw new Error($"ERROR LEXICO, Se esperaba cierre de comillas (\" \") en la linea {lineCount + 1}", log);
                    }

                    c = (char)archivo.Read();

                    if (c == '"')  // SI ENCUENTRA CIERRE DE COMILLAS TERMINAR WHILE
                    {
                        buffer += c;
                        break;
                    }

                    buffer += c;  // CONCATENAR CIERRE DE COMILLAS
                }
            }


            //********************* OPERADOR CADENA FIN **********************

            //********************* OPERADOR CARACTER INICIO ********************
            else if (c == '#') //SIGNO DE GATO
            {
                setClasificacion(Tipos.Caracter);
                if (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                    while (char.IsDigit(c = (char)archivo.Peek()))
                    {
                        buffer += c;
                        archivo.Read();
                    }

                }

            }

            else if (c == '@') // CARACTE ARROBA
            {
                setClasificacion(Tipos.Caracter);
            }

            else if (c == '\'') // CARACTER ENTRE COMILLAS SIMPLES
            {
                setClasificacion(Tipos.Caracter);
                c = (char)archivo.Read();
                buffer += c;
                c = (char)archivo.Read();
                if (c != '\'') //VALIDAR SI HAY MAS DE UN CARACTER
                {
                    if (char.IsWhiteSpace(c = (char)archivo.Peek()))// IGNORAR LOS ESPACIOS
                    {
                        c = (char)archivo.Read();
                    }
                    else
                    {   //SI HAY MAS DE UN CARACTER ENTRE COMILLAS SIMPLES LANZAR ERROR
                        throw new Error($"ERROR LEXICO, Se esperaba solo un caracter entre comillas simples (\' \') en la linea {lineCount + 1}", log);
                    }

                }
                while (c != '\'')
                {
                    if (finArchivo())
                    {   // SI NO ENCUENTRA EL CIERRE DE COMILLAS SIMPLES AL FINALIZAR ARCHIVO LANZAR ERROR
                        throw new Error($"ERROR LEXICO, Se esperaba cierre de comillas simples (\' \') en la linea {lineCount + 1}", log);
                    }

                    c = (char)archivo.Read();

                    if (c == '\'')
                    {
                        buffer += c;
                    }
                }
                buffer += c;// CONCATENAR COMILLAS FINALES
            }

            //********************* OPERADOR CARACTER FIN **********************


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