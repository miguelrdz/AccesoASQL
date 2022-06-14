using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace AccesoASQL
{
    public class ConsultasASQL
    {
        private string _stringDeConexion;
        private List<SqlParameter> _coleccionDeParametros;
        public ConsultasASQL()
        {
            _stringDeConexion = "Data Source=TERNP315591\\SANDBOX;Initial Catalog=Desarrollo;Integrated Security=True;TrustServerCertificate=True";
            _coleccionDeParametros = new List<SqlParameter>();
        }
        public string StringDeConexion
        {
            get { return _stringDeConexion; }
        }
        public void AgregarParametro(Object valorRecibido, Type tipoDato, string nombreParametro)
        {
            try
            {
                if (valorRecibido != null)
                {
                    SqlParameter _parametro = new SqlParameter
                    {
                        ParameterName = "@" + nombreParametro,
                        Value = valorRecibido
                    };
                    switch (tipoDato.Name)
                    {
                        case "Decimal":
                            _parametro.SqlDbType = System.Data.SqlDbType.Decimal;
                            break;
                        case "Double":
                            _parametro.SqlDbType = System.Data.SqlDbType.BigInt;
                            break;
                        case "String":
                            _parametro.SqlDbType = System.Data.SqlDbType.NVarChar;
                            break;
                        default:
                            throw new InvalidCastException("El tipo de dato no está considerado en la lista de datos válidos");
                    }
                    _coleccionDeParametros.Add(_parametro);
                }
                else
                {
                    throw new InvalidOperationException("El valor recibido no puede ser nulo");
                }
            }
            catch (Exception _e)
            {
                this.RegistraError(_e);
            }
        }
        public void EjecutaSP(string SpAEjecutarString)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            using (SqlConnection conn = new SqlConnection(_stringDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand(SpAEjecutarString, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 15;
                    if (_coleccionDeParametros.Count > 0)
                    {
                        cmd.Parameters.AddRange(_coleccionDeParametros.ToArray());
                        _coleccionDeParametros = new List<SqlParameter>();
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    
                }
            }
            return;
        }
        public void RegistraError(Exception mensajeDeError)
        {
            ConsultasASQL _GrabaError = new ConsultasASQL();
            _GrabaError.AgregarParametro(mensajeDeError.Message.ToString(), typeof(string), "MensajeARegistrar");
            _GrabaError.EjecutaSP("RegistraLog"); 
            _GrabaError.AgregarParametro(mensajeDeError.StackTrace.ToString(), typeof(string), "MensajeARegistrar");
            _GrabaError.EjecutaSP("RegistraLog");
            throw new Exception(mensajeDeError.ToString());
        }
    }
}
