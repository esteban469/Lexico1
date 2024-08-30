using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
/*
 Requerimientos
 1.- Sobrecargar el constructor lexico para que reciba como argumento el nombre del archivo a compilar 
 2.- Tener contador de lineas 
*/
namespace Lexico1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea;
        public Lexico()
        {
            linea = 1;
            log     = new StreamWriter("prueba.log");
            asm     = new StreamWriter("prueba.asm");
            log.AutoFlush=true;
            asm.AutoFlush=true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe",log);
            }
        }
        /*public Lexico();
        {
            
            Si nombre = suma.cpp
            LOG=nombre.log
            ASM=suma.asm
            Y validar la extension del nombre del archivo 
            
        } */
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }
        public void nextToken()
        {
            char c;
            String buffer="";
            while(char.IsWhiteSpace(c = (char)archivo.Read()))
            {

            }
            if(char.IsLetter(c))
            {
                SetClasificacion(Tipos.Identificador);
            }
            else if(char.IsDigit(c))
            {
                SetClasificacion(Tipos.Numero);
            }
            else
            {
                SetClasificacion(Tipos.Caracter);
            }
            SetContenido(buffer);
            log.WriteLine(GetContenido() + " = " + GetClasificacion());

            archivo.Read();
            archivo.Peek();
        }
    }
}