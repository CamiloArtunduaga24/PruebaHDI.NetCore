using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Prueba.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            // Conectarse a la base de datos
            var connectionString = "Data Source=localhost;Initial Catalog=myDatabase;Integrated Security=True";
            var connection = new SqlConnection(connectionString);
            connection.Open();

            // Consultar los datos de la tabla
            var query = "SELECT * FROM Riesgos";
            var command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();

            // Convertir los datos a JSON
            var riesgos = new List<Riesgo>();
            while (reader.Read())
            {
                riesgos.Add(new Riesgo
                {
                    Score = reader["Score"]?.ToString(),
                    Informacion = new Informacion
                    {
                        Riesgo = new Riesgo
                        {
                            fechaConsulta = reader["fechaConsulta"]?.ToString(),
                            secuencia = reader["secuencia"]?.ToString(),
                            codigoRespuesta = reader["codigoRespuesta"]?.ToString(),
                            Persona = new Persona
                            {
                                identificacion = new Identificacion
                                {
                                    numero = reader["identificacionNumero"]?.ToString(),
                                    tipo = reader["identificacionTipo"]?.ToString(),
                                    estadi = reader["identificacionEstadi"]?.ToString(),
                                    ciudad = reader["identificacionCiudad"]?.ToString(),
                                    departamento = reader["identificacionDepartamento"]?.ToString(),
                                    genero = reader["identificacionGenero"]?.ToString()
                                },
                                edad = new Edad
                                {
                                    min = reader["edadMin"]?.ToString(),
                                    max = reader["edadMax"]?.ToString()
                                },
                                nombres = reader["nombres"]?.ToString(),
                                primerApellido = reader["primerApellido"]?.ToString(),
                                segundoApellido = reader["segundoApellido"]?.ToString()
                            }
                        },
                        Categoria = new Categoria
                        {
                            vin = reader["categoriaVin"]?.ToString(),
                            tipoDocumento = reader["categoriaTipoDocumento"]?.ToString(),
                            numeroDocumento = reader["categoriaNumeroDocumento"]?.ToString(),
                            fechaUltimaCarga = reader["categoriaFechaUltimaCarga"]?.ToString(),
                            variablesScr = reader["categoriaVariablesScr"]?.ToString().Split(";").ToList(),
                            variablesRes = reader["categoriaVariablesRes"]?.ToString().Split(";").ToList().Select(x => new VariableRes
                            {
                                nombreVariable = x.Split(",")[0],
                                scoreVariable = x.Split(",")[1],
                                descripcion = x.Split(",")[2]
                            })
                        }
                    }
                });
            }

            // Cerrar la conexión a la base de datos
            connection.Close();

            // Retornar los datos
            return Json(riesgos);
        }
    }

    public class Riesgo
    {
        public string Score { get; set; }
        public Informacion Informacion { get; set; }
    }

    public class Informacion
    {
        public Riesgo Riesgo { get; set; }
        public Categoria Categoria { get; set; }
    }

    public class Riesgo
    {
        public string fechaConsulta { get; set; }
        public string secuencia { get; set; }
        public string codigoRespuesta { get; set; }
        public Persona Persona { get; set; }