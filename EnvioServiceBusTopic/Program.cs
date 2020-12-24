using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace EnvioServiceBusTopic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();
            logger.Information(
                "Testando o envio de mensagens para um Topico do Azure Service Bus");

            if (args.Length < 3)
            {
                logger.Error(
                    "Informe ao menos 3 parametros: " +
                    "no primeiro a string de conexao com o Azure Service Bus, " +
                    "no segundo o Topico a que recebera as mensagens, " +
                    "ja no terceiro em diante as mensagens a serem " +
                    "enviadas ao Topico do Azure Service Bus...");
                return;
            }

            string connectionString = args[0];
            string nomeTopic = args[1];

            logger.Information($"Topic = {nomeTopic}");

            TopicClient client = null;
            try
            {
                client = new TopicClient(
                    connectionString, nomeTopic);
                for (int i = 2; i < args.Length; i++)
                {
                    await client.SendAsync(
                        new Message(Encoding.UTF8.GetBytes(args[i])));

                    logger.Information(
                        $"[Mensagem enviada] {args[i]}");
                }

                logger.Information("Concluido o envio de mensagens");
            }
            catch (Exception ex)
            {
                logger.Error($"Exceção: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");
            }
            finally
            {
                if (client is not null)
                {
                    await client.CloseAsync();
                    logger.Information(
                        "Conexao com o Azure Service Bus fechada!");
                }
                    
            }
        }
    }
}