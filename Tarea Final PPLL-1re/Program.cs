using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Tarea: Simulación de Tráfico con Autos y Semáforos

public class SimulacionTrafico
{
    static async Task Main(string[] args)
    {
        
        CancellationTokenSource cts = new CancellationTokenSource();

        Console.WriteLine("Iniciando simulación de tráfico...");

        
        var semaforo1 = Task.Factory.StartNew(() => ControlarSemaforo("Semáforo 1", cts.Token),
            cts.Token,
            TaskCreationOptions.AttachedToParent,
            TaskScheduler.Default);

        var semaforo2 = Task.Factory.StartNew(() => ControlarSemaforo("Semáforo 2", cts.Token),
            cts.Token,
            TaskCreationOptions.AttachedToParent,
            TaskScheduler.Default);

        
        var autos = new List<Task>
        {
            Task.Run(() => SimularAuto("Auto 1", cts.Token)),
            Task.Run(() => SimularAuto("Auto 2", cts.Token)),
            Task.Run(() => SimularAuto("Auto 3", cts.Token))
        };

        
        await Task.WhenAny(Task.WhenAny(autos), semaforo1, semaforo2).ContinueWith(t =>
        {
            Console.WriteLine("Una tarea ha terminado. Deteniendo simulación...");
            cts.Cancel(); 
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

        Console.WriteLine("La simulación ha terminado.");
    }

    
    private static async Task ControlarSemaforo(string nombreSemaforo, CancellationToken token)
    {
        var colores = new[] { "Rojo", "Verde", "Amarillo" };
        int indiceColor = 0;

        while (!token.IsCancellationRequested)
        {
            string colorActual = colores[indiceColor];
            Console.WriteLine($"{nombreSemaforo}: {colorActual}");

            
            int tiempoEspera = colorActual switch
            {
                "Rojo" => 3000,
                "Verde" => 5000,
                "Amarillo" => 2000,
                _ => 1000
            };

            await Task.Delay(tiempoEspera, token);

            
            indiceColor = (indiceColor + 1) % colores.Length;
        }
    }

    
    private static async Task SimularAuto(string nombreAuto, CancellationToken token)
    {
        Console.WriteLine($"{nombreAuto} está en camino...");

        while (!token.IsCancellationRequested)
        {
            int avance = new Random().Next(10, 50);
            Console.WriteLine($"{nombreAuto} avanza {avance} metros...");

            await Task.Delay(2000, token); 

            if (avance > 40) 
            {
                Console.WriteLine($"{nombreAuto} ha cruzado el semáforo. ¡Fin de la simulación!");
                break;
            }
        }
    }
}



