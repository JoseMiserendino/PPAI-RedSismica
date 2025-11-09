using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PPAI_V2.entidades;

namespace PPAI_V2
{
    internal class GestorRegistrarRM
    {
        // Atributos
        private InterfazRegistrarRM interfazRegistrarRM;
        private List<EventoSismico> eventosAutoDetectadosNoRevisados;
        private EventoSismico eventoSismicoSeleccionado;
        private Dictionary<string, object> datosSeriesTemporalesEventoSeleccionado;
        // private Estado estadoBloqueadoEnRevision;
        private String alcanceEventoSelec;
        private String clasificacionEventoSelec;
        private String origenEventoSelec;
        List<String> alcances;
        List<String> clasificaciones;
        List<String> origenes;

        private List<String> opcionesFinales = new List<String>
        {
            "Confirmar evento", 
            "Rechazar evento", 
            "Solicitar revisión a experto"
        };
        String opcFinalIngresada;
        //Estado estadoRechazado;

        // Objetos para llevar a cabo el CU (BBDD)
        private List<EventoSismico> eventosSismicos;
        private List<Estado> estados;
        private Empleado empleadoLogueado;
        private List<Sismografo> sismografos;
        private List<AlcanceSismo> alcancesSismos;
        private List<ClasificacionSismo> clasificacionesSismos;
        private List<OrigenDeGeneracion> origenesGeneracionSismos;

        // Constructor
        public GestorRegistrarRM(InterfazRegistrarRM interfazRegistrarRM)
        {
            // Guardar referencia a interfaz
            this.interfazRegistrarRM = interfazRegistrarRM;

            // Crear estados

            Estado estadoAutoDetectado = new AutoDetectado();
            Estado estadoAutoConfirmado = new AutoConfirmado();
            
            //Estado estadoAutoConfirmado = new Estado("EventoSismico", "AutoConfirmado");
            //Estado estadoAutoDetectado = new Estado("EventoSismico", "AutoDetectado");
            //Estado estadoRechazado = new Estado("EventoSismico", "Rechazado");
            //Estado estadoBloqueadoEnRevision = new Estado("EventoSismico", "BloqueadoEnRevision");

            estados = new List<Estado>
            {
                estadoAutoConfirmado,
                estadoAutoDetectado,
            };

            // Crear empleado
            Empleado empleado1 = new Empleado("Perez", "Juan", "juanperez@gmail.com", "3515555555");

            // Crear sesion
            Sesion.IniciarSesion(empleado1);

            // Crear cambios de estados
            CambioEstado cambioEstado1 = new CambioEstado(
                new DateTime(2025, 1, 1, 12, 0, 0), // Fecha de inicio: Año, Mes, Día, Hora, Minuto, Segundo
                estadoAutoDetectado,
                empleado1
                );

            CambioEstado cambioEstado2 = new CambioEstado(
                new DateTime(2025, 1, 1, 17, 15, 0),
                estadoAutoDetectado,
                empleado1
                );

            CambioEstado cambioEstado3 = new CambioEstado(
                new DateTime(2025, 1, 5, 15, 30, 0),
                estadoAutoConfirmado, empleado1);

            // Crear clasificacion de sismos
            ClasificacionSismo clasificacionSismo1 = new ClasificacionSismo("Superficial", 0, 70);
            ClasificacionSismo clasificacionSismo2 = new ClasificacionSismo("Intermedio", 70, 300);
            ClasificacionSismo clasificacionSismo3 = new ClasificacionSismo("profundo", 300, 700);

            clasificacionesSismos = new List<ClasificacionSismo>
            {
                clasificacionSismo1,
                clasificacionSismo2,
                clasificacionSismo3
            };

            // Crear Magnitud Richter
            MagnitudRichter magnitudRichter1 = new MagnitudRichter("Magnitud 1", 1.2);
            MagnitudRichter magnitudRichter2 = new MagnitudRichter("Magnitud 2", 2.4);
            MagnitudRichter magnitudRichter3 = new MagnitudRichter("Magnitud 3", 3.7);
            MagnitudRichter magnitudRichter4 = new MagnitudRichter("Magnitud 4", 5.3);

            // Crear Origen de Generacion
            OrigenDeGeneracion origenGeneracion1 = new OrigenDeGeneracion("Tectonico", "Movimientos de las placas tectónicas en la corteza terrestre, causados por la acumulación y liberación de energía en fallas geológicas");
            OrigenDeGeneracion origenGeneracion2 = new OrigenDeGeneracion("Volcanico", "Actividad volcánica, como el movimiento de magma, gases o fracturamiento de roca en cámaras magmáticas.");
            OrigenDeGeneracion origenGeneracion3 = new OrigenDeGeneracion("Inducido", "Actividades humanas, como la extracción de petróleo o gas, la inyección de fluidos en el subsuelo (fracking), la construcción de embalses o la minería.");

            origenesGeneracionSismos = new List<OrigenDeGeneracion>
            {
                origenGeneracion1,
                origenGeneracion2,
                origenGeneracion3
            };

            // Crear Alcance Sismo
            AlcanceSismo alcanceSismo1 = new AlcanceSismo("Local", "Afecta una región geográfica limitada, como una ciudad o un área metropolitana.");
            AlcanceSismo alcanceSismo2 = new AlcanceSismo("Regional", "Afectan una región más amplia, abarcando decenas o cientos de kilómetros desde el epicentro.");
            AlcanceSismo alcanceSismo3 = new AlcanceSismo("Telurico", "Pueden sentirse a cientos o miles de kilómetros del epicentro, afectando grandes regiones o incluso países.");

            alcancesSismos = new List<AlcanceSismo>
            {
                alcanceSismo1,
                alcanceSismo2,
                alcanceSismo3
            };

            // Crear Tipos de datos
            TipoDeDato velocidadOnda = new TipoDeDato("Velocidad de onda", "kilómetros por segundo", 6.0);
            TipoDeDato frecuenciaOnda = new TipoDeDato("Frecuencia de onda", "hercios", 1.0);
            TipoDeDato longitudOnda = new TipoDeDato("Longitud de onda", "kilómetros por ciclo", 6.0);

            // Crear detalles de muestras

            // Para primera serie temporal
            DetalleMuestraSismica detalleMuestraSismica1 = new DetalleMuestraSismica(6.0, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica2 = new DetalleMuestraSismica(1.0, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica3 = new DetalleMuestraSismica(6.0, longitudOnda);

            // Para segunda serie temporal
            DetalleMuestraSismica detalleMuestraSismica4 = new DetalleMuestraSismica(6.5, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica5 = new DetalleMuestraSismica(1.2, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica6 = new DetalleMuestraSismica(5.8, longitudOnda);

            // Para tercera serie temporal
            DetalleMuestraSismica detalleMuestraSismica16 = new DetalleMuestraSismica(5.9, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica17 = new DetalleMuestraSismica(1.0, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica18 = new DetalleMuestraSismica(5.7, longitudOnda);

            // Para primera serie temporal de otro evento
            DetalleMuestraSismica detalleMuestraSismica7 = new DetalleMuestraSismica(5.8, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica8 = new DetalleMuestraSismica(0.9, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica9 = new DetalleMuestraSismica(5.5, longitudOnda);

            // Para segunda serie temporal de otro evento
            DetalleMuestraSismica detalleMuestraSismica10 = new DetalleMuestraSismica(6.2, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica11 = new DetalleMuestraSismica(1.1, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica12 = new DetalleMuestraSismica(6.1, longitudOnda);

            // Para tercera serie temporal de otro evento
            DetalleMuestraSismica detalleMuestraSismica19 = new DetalleMuestraSismica(5.9, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica20 = new DetalleMuestraSismica(1.0, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica21 = new DetalleMuestraSismica(5.7, longitudOnda);

            // Para tercer evento
            DetalleMuestraSismica detalleMuestraSismica13 = new DetalleMuestraSismica(5.9, velocidadOnda);
            DetalleMuestraSismica detalleMuestraSismica14 = new DetalleMuestraSismica(1.0, frecuenciaOnda);
            DetalleMuestraSismica detalleMuestraSismica15 = new DetalleMuestraSismica(5.7, longitudOnda);


            // Crear muestras sismicas
            // Para evento 1
            MuestraSismica muestraSismica1 = new MuestraSismica(new DateTime(2025, 1, 1, 12, 0, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica1, detalleMuestraSismica2, detalleMuestraSismica3 });
            MuestraSismica muestraSismica2 = new MuestraSismica(new DateTime(2025, 1, 1, 12, 10, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica4, detalleMuestraSismica5, detalleMuestraSismica6 });
            MuestraSismica muestraSismica3 = new MuestraSismica(new DateTime(2025, 1, 1, 12, 0, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica16 , detalleMuestraSismica17 , detalleMuestraSismica18 });
            
            // Para evento 2
            MuestraSismica muestraSismica4 = new MuestraSismica(new DateTime(2025, 1, 1, 17, 15, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica10, detalleMuestraSismica11, detalleMuestraSismica12 });
            MuestraSismica muestraSismica5 = new MuestraSismica(new DateTime(2025, 1, 1, 17, 10, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica7, detalleMuestraSismica8, detalleMuestraSismica9 });
            MuestraSismica muestraSismica6 = new MuestraSismica(new DateTime(2025, 1, 1, 17, 15, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica19, detalleMuestraSismica20, detalleMuestraSismica21 });

            // Para evento 3
            MuestraSismica muestraSismica7 = new MuestraSismica(new DateTime(2025, 1, 5, 15, 30, 0), new List<DetalleMuestraSismica> { detalleMuestraSismica13, detalleMuestraSismica14, detalleMuestraSismica15 });


            // Crear serie temporal de evento 1
            SerieTemporal serieTemporal1 = new SerieTemporal(
                1.0, // condicionAlarma: valor umbral para disparar una alarma (por ejemplo, frecuencia límite en Hz)
                new DateTime(2025, 1, 1, 12, 0, 0), // fechaHoraInicioRegistroMuestras
                new DateTime(2025, 1, 1, 12, 5, 0), // fechaHoraFinRegistro: 5 minutos después
                0.1, // frecuenciaMuestreo: 0.1 Hz (una muestra cada 10 segundos)
                new List<MuestraSismica> { muestraSismica1 } // muestrasSismicas
            );

            SerieTemporal serieTemporal2 = new SerieTemporal(
                1.2, 
                new DateTime(2025, 1, 1, 12, 2, 0), 
                new DateTime(2025, 1, 1, 12, 7, 0), 
                0.2, 
                new List<MuestraSismica> { muestraSismica2 } 
            );

            SerieTemporal serieTemporal6 = new SerieTemporal(
                1.0, 
                new DateTime(2025, 1, 1, 12, 3, 0), 
                new DateTime(2025, 1, 1, 12, 8, 0),
                0.1, 
                new List<MuestraSismica> { muestraSismica3 } 
            );

            // Serie temporal de evento 2
            SerieTemporal serieTemporal3 = new SerieTemporal(
                1.0, 
                new DateTime(2025, 1, 1, 17, 10, 0), 
                new DateTime(2025, 1, 1, 17, 15, 0), 
                0.1, 
                new List<MuestraSismica> { muestraSismica4 }
            );

            SerieTemporal serieTemporal4 = new SerieTemporal(
                1.2, 
                new DateTime(2025, 1, 1, 17, 11, 0), 
                new DateTime(2025, 1, 1, 17, 16, 0), 
                0.2, 
                new List<MuestraSismica> { muestraSismica5 }
            );

            SerieTemporal serieTemporal7 = new SerieTemporal(
                1.0, 
                new DateTime(2025, 1, 1, 17, 12, 0), 
                new DateTime(2025, 1, 1, 17, 17, 0), 
                0.1, 
                new List<MuestraSismica> { muestraSismica6 }
            );

            // Serie temporal de evento 3
            SerieTemporal serieTemporal5 = new SerieTemporal(
                2.0,
                new DateTime(2025, 1, 5, 15, 30, 0), // fechaHoraInicioRegistroMuestras
                new DateTime(2025, 1, 5, 15, 35, 0), // fechaHoraFinRegistro: 5 minutos después
                0.1, // frecuenciaMuestreo: 0.1 Hz (una muestra cada 10 segundos)
                new List<MuestraSismica> { muestraSismica7 } // muestrasSismicas
                );


            // Crear eventos sismicos. Van a ser 2
            EventoSismico eventoSismico1 = new EventoSismico(
                new DateTime(2025, 1, 1, 12, 0, 0),
                new DateTime(2025, 1, 1, 12, 5, 0),
                -31.578898,
                -65.514715,
                -31.612283,
                -65.717223,
                1.2,
                clasificacionSismo1,
                magnitudRichter1,
                origenGeneracion1,
                alcanceSismo1,
                estadoAutoDetectado,
                new List<CambioEstado> { cambioEstado1 },
                new List<SerieTemporal> { serieTemporal1, serieTemporal2, serieTemporal6 }
                );

            EventoSismico eventoSismico2 = new EventoSismico(
                new DateTime(2025, 1, 1, 17, 15, 0),
                new DateTime(2025, 1, 1, 17, 20, 0),
                -30.540950,
                -65.237950,
                -30.643180,
                -65.040383,
                2.4,
                clasificacionSismo2,
                magnitudRichter2,
                origenGeneracion2,
                alcanceSismo1,
                estadoAutoDetectado,
                new List<CambioEstado> { cambioEstado2 },
                new List<SerieTemporal> { serieTemporal3, serieTemporal4, serieTemporal7 }
                );

            EventoSismico eventoSismico3 = new EventoSismico(
                new DateTime(2025, 1, 5, 15, 30, 0),
                new DateTime(2025, 1, 5, 15, 35, 0),
                -30.224104,
                -69.275174,
                -29.599504,
                -69.700139,
                5.3,
                clasificacionSismo1,
                magnitudRichter4,
                origenGeneracion1,
                alcanceSismo1,
                estadoAutoConfirmado,
                new List<CambioEstado> { cambioEstado3 },
                new List<SerieTemporal> { serieTemporal5 }
                );

            eventosSismicos = new List<EventoSismico>
            {
                eventoSismico1,
                eventoSismico2,
                eventoSismico3
            };

            // Crear estaciones sismologicas y sismógrafos
            EstacionSismologica estacionSanJuan = new EstacionSismologica(
        "SJ001", "CERT_SJ_2020", new DateTime(2020, 1, 1), -31.5351, -68.5251, "San Juan", 1001);

            EstacionSismologica estacionMendoza = new EstacionSismologica(
                "MZ001", "CERT_MZ_2020", new DateTime(2020, 1, 1), -32.8895, -68.8458, "Mendoza", 1002);

            
            EstacionSismologica estacionLaRioja = new EstacionSismologica(
                "LR001", "CERT_LR_2020", new DateTime(2020, 1, 1), -29.4138, -66.8558, "La Rioja", 1003);

            sismografos = new List<Sismografo>
            {
                new Sismografo(new DateTime(2020, 6, 1), "SJ01", "SER001",
                    estacionSanJuan,
                    new List<SerieTemporal> { serieTemporal1, serieTemporal3, serieTemporal5 }),

                new Sismografo(new DateTime(2020, 6, 1), "MZ01", "SER002", estacionMendoza,
                new List<SerieTemporal> {serieTemporal2, serieTemporal4}),

                new Sismografo(new DateTime(2020, 6, 1), "LR01", "SER003", estacionLaRioja, 
                new List<SerieTemporal> { serieTemporal6, serieTemporal7 })
            };
        }

        public void OpcRegistrarResultadoRM()
        {
            TomarEventosAutoDetectadosNoRevisados();
        }

        public void TomarEventosAutoDetectadosNoRevisados()
        {
            List<EventoSismico> eventos = new List<EventoSismico>();
            foreach (var evento in eventosSismicos)
            {
                if (evento.EsAutoDetectado())
                {
                    eventos.Add(evento);
                }
            }

            eventosAutoDetectadosNoRevisados = eventos;

            OrdenarEventos();
        }

        public void OrdenarEventos()
        {
            eventosAutoDetectadosNoRevisados.Sort((x, y) => x.FechaHoraOcurrencia.CompareTo(y.FechaHoraOcurrencia));
            TomarDatosPrincipalesEventos();
        }

        public void TomarDatosPrincipalesEventos()
        {
            List<string> datosPrincipales = new List<string>();
            foreach (var evento in eventosAutoDetectadosNoRevisados)
            {
                datosPrincipales.Add(evento.TomarDatosPrincipales());
            }

            interfazRegistrarRM.PedirSeleccionEventoNoRevisado(datosPrincipales);
        }

        public void TomarSelecEventoNoRevisado(int index)
        {
            if (index >= 0 && index < eventosAutoDetectadosNoRevisados.Count)
            {
                eventoSismicoSeleccionado = eventosAutoDetectadosNoRevisados[index];
                Console.WriteLine($"Evento seleccionado: {eventoSismicoSeleccionado.TomarDatosPrincipales()}");
            } else
            {
                throw new Exception($"Hubo un error al seleccionar un evento no revisado");
            }

            DateTime fechaHoraActual = TomarFechaYHoraActual();
            TomarUsuarioLogueado();
            BloquearEventoSelec(fechaHoraActual);
            TomarDatosSismicosEventoSelec();
        }

        //public void BuscarEstadoBloqueadoEnRevision()
        //{
        //    foreach (var estado in estados)
        //    {
        //        if (estado.EsAmbitoEventoSismico() && estado.EsBloqueadoEnRevision())
        //        {
        //            Console.WriteLine($"Estado bloqueado en revisión encontrado: {estado.NombreEstado}");
        //            estadoBloqueadoEnRevision = estado;
        //            break;
        //        }
        //    }

        //    TomarUsuarioLogueado();
        //}

        public DateTime TomarFechaYHoraActual()
        { 
            return DateTime.Now;
        }

        public void TomarUsuarioLogueado()
        {
            empleadoLogueado = Sesion.IniciarSesion(empleadoLogueado).ObtenerUsuarioLogueado();
        }

        public void BloquearEventoSelec(DateTime fh)
        {
            eventoSismicoSeleccionado.BloquearEvento(fh, empleadoLogueado);
            //eventoSismicoSeleccionado.BloquearEvento(estadoBloqueadoEnRevision, empleadoLogueado);
            //Console.WriteLine($"Evento Bloqueado: \n{eventoSismicoSeleccionado}");
            //TomarDatosSismicosEventoSelec();
        }

        public void TomarDatosSismicosEventoSelec()
        {

            var (alcance, clasificacion, origen) = eventoSismicoSeleccionado.BuscarDatosSismicosEventoSelec();

            alcanceEventoSelec = alcance;
            clasificacionEventoSelec = clasificacion;
            origenEventoSelec = origen;

            String datosAlcanceClasificacionOrigenEventoSeleccionado = $"Alcance: {alcanceEventoSelec}\n" +
                        $"Clasificación: {clasificacionEventoSelec}\n" +
                        $"Origen de Generación: {origenEventoSelec}";

            Console.WriteLine($"Datos del evento seleccionado:\n{datosAlcanceClasificacionOrigenEventoSeleccionado}");

            interfazRegistrarRM.MostrarDatosSismicosEventoSelec(datosAlcanceClasificacionOrigenEventoSeleccionado);
        }

        public void TomarConfirmacionEvento()
        {
            TomarDatosSeriesTemporalesEvento();
        }

        public void TomarDatosSeriesTemporalesEvento()
        {
            var resultado = new Dictionary<string, object>();
            var datosEstaciones = new List<Dictionary<string, object>>();

            if (eventoSismicoSeleccionado == null || eventoSismicoSeleccionado.SerieTemporal == null ||
                !eventoSismicoSeleccionado.SerieTemporal.Any())
            {
                Console.WriteLine("No hay series temporales asociadas al evento.");
            }

            foreach (var sismografo in sismografos)
            {
                var seriesRelevantes = sismografo.ObtenerSeriesRelevantes(eventoSismicoSeleccionado.SerieTemporal);

                if (!seriesRelevantes.Any())
                    continue;

                var nombreEstacion = sismografo.TomarNombreEstacion();
                var identificadorSismografo = sismografo.IdentificadorSismografo;


                var datosEstacion = new Dictionary<string, object>
                {
                    ["NombreEstacion"] = nombreEstacion,
                    ["Sismografo"] = identificadorSismografo,
                    ["SeriesTemporales"] = new List<Dictionary<string, object>>()
                };

                foreach (var serie in seriesRelevantes)
                {
                    var datosSerie = new Dictionary<string, object>
                    {
                        ["FechaInicio"] = serie.FechaHoraInicioRegistroMuestras,
                        ["FechaFin"] = serie.FechaHoraFinRegistro,
                        ["Muestras"] = new List<Dictionary<string, object>>()
                    };

                    foreach (var muestra in serie.MuestrasSismicas)
                    {
                        // Obtener los detalles de la muestra usando el método existente
                        var detalles = muestra.GetDatos();
                        var datosMuestra = new Dictionary<string, object>
                        {
                            ["FechaHora"] = muestra.FechaHoraMuestra,
                            ["Detalles"] = detalles
                        };

                        ((List<Dictionary<string, object>>)datosSerie["Muestras"]).Add(datosMuestra);
                    }

                    ((List<Dictionary<string, object>>)datosEstacion["SeriesTemporales"]).Add(datosSerie);
                }

                datosEstaciones.Add(datosEstacion);
            }

            resultado["Estaciones"] = datosEstaciones;

            // Mostrar el objeto en consola
            MostrarDatosEnConsola(resultado);

            // Continuar con el flujo
            LlamarCUGenerarSismograma();

            datosSeriesTemporalesEventoSeleccionado = resultado;
        }

        private void MostrarDatosEnConsola(Dictionary<string, object> datos)
        {
            Console.WriteLine("\nDatos estructurados de series temporales:");
        
            if (!datos.ContainsKey("Estaciones")) return;
        
            var estaciones = (List<Dictionary<string, object>>)datos["Estaciones"];
        
            foreach (var estacion in estaciones)
            {
                Console.WriteLine($"\nEstación: {estacion["NombreEstacion"]}, Sismógrafo: {estacion["Sismografo"]}");
        
                var series = (List<Dictionary<string, object>>)estacion["SeriesTemporales"];
                foreach (var serie in series)
                {
                    Console.WriteLine($"  Serie Temporal desde {((DateTime)serie["FechaInicio"]):yyyy-MM-dd HH:mm:ss} hasta {((DateTime)serie["FechaFin"]):yyyy-MM-dd HH:mm:ss}");
        
                    var muestras = (List<Dictionary<string, object>>)serie["Muestras"];
                    foreach (var muestra in muestras)
                    {
                        Console.WriteLine($"    {muestra["Detalles"]}");
                    }
                }
            }
        }


        public void LlamarCUGenerarSismograma()
        {
            interfazRegistrarRM.MostrarSismograma();
        }

        public void TomarConfirmacionSismograma()
        {
            HabilitarOpcVisualizarMapa();
        }

        public void HabilitarOpcVisualizarMapa()
        {
            interfazRegistrarRM.HabilitarOpcVisualizarMapa();
        }

        public void TomarSelecVisualizacionMapa(bool seleccion)
        {
            if (seleccion)
            {
                Console.WriteLine("Ver mapa");
            } else
            {
                Console.WriteLine("Rechazar ver mapa");
                HabilitarModificacionEvento();
            }
        }

        public void HabilitarModificacionEvento()
        {
            alcances = BuscarAlcances();
            clasificaciones = BuscarClasificaciones();
            origenes = BuscarOrigenesGeneracion();

            interfazRegistrarRM.PedirModificacionEvento(alcanceEventoSelec, clasificacionEventoSelec, origenEventoSelec, alcances, clasificaciones, origenes);
        }

        public List<string> BuscarAlcances()
        {
            List<string> alcances = new List<string>();

            foreach (var alcance in alcancesSismos)
            {
                alcances.Add(alcance.Nombre);
            }
            ;
            return alcances;
        }

        public List<String> BuscarClasificaciones()
        {
            List<string> clasificaciones = new List<string>();
            foreach (var clasificacion in clasificacionesSismos)
            {
                clasificaciones.Add(clasificacion.Nombre);
            }
            ;
            return clasificaciones;
        }

        public List<String> BuscarOrigenesGeneracion()
        {
            List<string> origenes = new List<string>();
            foreach (var origen in origenesGeneracionSismos)
            {
                origenes.Add(origen.Nombre);
            }
            ;
            return origenes;
        }







        public void TomarModificacionEvento(string alcance, string clasificacion, string origen)
    {
        // Actualizar las variables de tipo string (manteniendo tu lógica existente)
        alcanceEventoSelec = alcance;
        clasificacionEventoSelec = clasificacion;
        origenEventoSelec = origen;

        // Verificar que eventoSismicoSeleccionado no sea null
        if (eventoSismicoSeleccionado == null)
        {
            Console.WriteLine("Error: No hay un evento sísmico seleccionado.");
            return;
        }

        // Buscar y asignar el objeto AlcanceSismo correspondiente
        AlcanceSismo nuevoAlcance = alcancesSismos.FirstOrDefault(a => a.Nombre.Equals(alcance, StringComparison.OrdinalIgnoreCase));
        if (nuevoAlcance != null)
        {
            eventoSismicoSeleccionado.AlcanceSismo = nuevoAlcance;
            Console.WriteLine($"Alcance actualizado a: {nuevoAlcance.Nombre}");
        }
        else
        {
            Console.WriteLine($"Error: No se encontró el alcance '{alcance}' en la lista de alcances posibles.");
        }

        // Buscar y asignar el objeto ClasificacionSismo correspondiente
        ClasificacionSismo nuevaClasificacion = clasificacionesSismos.FirstOrDefault(c => c.Nombre.Equals(clasificacion, StringComparison.OrdinalIgnoreCase));
        if (nuevaClasificacion != null)
        {
            eventoSismicoSeleccionado.Clasificacion = nuevaClasificacion;
            Console.WriteLine($"Clasificación actualizada a: {nuevaClasificacion.Nombre}");
        }
        else
        {
            Console.WriteLine($"Error: No se encontró la clasificación '{clasificacion}' en la lista de clasificaciones posibles.");
        }

        // Buscar y asignar el objeto OrigenDeGeneracion correspondiente
        OrigenDeGeneracion nuevoOrigen = origenesGeneracionSismos.FirstOrDefault(o => o.Nombre.Equals(origen, StringComparison.OrdinalIgnoreCase));
        if (nuevoOrigen != null)
        {
            eventoSismicoSeleccionado.OrigenGeneracion = nuevoOrigen;
            Console.WriteLine($"Origen actualizado a: {nuevoOrigen.Nombre}");
        }
        else
        {
            Console.WriteLine($"Error: No se encontró el origen '{origen}' en la lista de orígenes posibles.");
        }

        // Continuar con el flujo, solicitando la selección de la opción final
        interfazRegistrarRM.PedirSelecOpcFinal(opcionesFinales);
    }

        

        public void TomarSelecOpcFinal(String opcion)
        {
            Console.WriteLine($"Opcion seleccionada: {opcion}");
            switch (opcion)
            {
                case "Confirmar evento":
                    opcFinalIngresada = "Confirmar evento";
                    break;
                case "Rechazar evento":
                    opcFinalIngresada = "Confirmar evento";
                    Console.WriteLine("Rechazar evento");
                    ValidarDatosEvento();
                    break;
                case "Solicitar revisión a experto": 
                opcFinalIngresada = "Confirmar evento";
                Console.WriteLine("Solicitar revision evento");
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }

        public void ValidarDatosEvento()
        {
            if (string.IsNullOrEmpty(alcanceEventoSelec) || string.IsNullOrEmpty(clasificacionEventoSelec) || string.IsNullOrEmpty(origenEventoSelec))
            {
                Console.WriteLine("Datos del evento incompletos. No se puede confirmar.");
                return;
            }
            
            ValidarOpcIngresada();
        }

        public void ValidarOpcIngresada()
        {
            if (string.IsNullOrEmpty(opcFinalIngresada))
            {
                Console.WriteLine("No se ingreso una opcion final. No se puede confirmar.");
                return;
            }

            ActualizarEstado();
        }

        //public void BuscarEstadoRechazado()
        //{
        //    foreach (var estado in estados)
        //    {
        //        if (estado.EsAmbitoEventoSismico() && estado.EsRechazado())
        //        {
        //            estadoRechazado = estado;
        //            Console.WriteLine($"Estado rechazado encontrado: {estadoRechazado.NombreEstado}");
        //            break;
        //        }
        //        ;
        //    }

        //    if (estadoRechazado != null)
        //    {
        //        ActualizarEstado();
        //    } else
        //    {
        //        Console.WriteLine("No se encontró un estado rechazado válido.");
        //    }
        //}

        public void ActualizarEstado()
        {
            DateTime fhActual = TomarFechaYHoraActual();
            eventoSismicoSeleccionado.rechazar(fhActual, empleadoLogueado);
            FinCU();
        }

        public void FinCU()
        {
            interfazRegistrarRM.FinCU();
        }
    }

}
