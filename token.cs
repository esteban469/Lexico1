using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico1
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, Caracter, FinSentencia, InicioBloque, FinBloque, OperadorTermino, OperadorTernario,
            OperadorFactor, IncrementoTermino, IncrementoFactor, Puntero, Asignacion, OperadorRelacional, OperadorLogico, moneda, Cadena
            

        }
        public string contenido;
        public Tipos clasificacion;
        public Token()
        {
            contenido = "";
            clasificacion = Tipos.Identificador;
        }
        public void setContenido(string contenido)
        {
            this.contenido = contenido;
        }
        public void setClasificacion(Tipos clasificacion)
        {
            this.clasificacion = clasificacion;
        }
        public string getContenido()
        {
            return this.contenido;
        }
        public Tipos getClasificacion()
        {
            return this.clasificacion;
        }
    }
}