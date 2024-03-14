using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.Client.Results;
using HiveMQtt.MQTT5.ReasonCodes;
using System.Text.Json;

namespace ClientHiveMQ
{
    public partial class Form1 : Form
    {
        HiveMQClient client;
        Dispositivo dev = new Dispositivo();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConexionBroker();
        }

        private async void ConexionBroker()
        {
            var options = new HiveMQClientOptions();
            options.Host = "4836bdded3434bc1b55eff77116d15f1.s1.eu.hivemq.cloud";
            options.Port = 8883;
            options.UserName = "Cliente1";
            options.Password = "Cliente1$";

            client = new HiveMQClient(options);

            ConnectResult connectResult = await client.ConnectAsync().ConfigureAwait(false);

            if (connectResult.ReasonCode == ConnAckReasonCode.Success)
            {
                MessageBox.Show("Conexión realizada con exito con el Broker", "Conexión Existosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Conexión no realizada con el Broker", "Conexión Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            client.OnMessageReceived += (sender, args) =>
            {
                
                string received_message = args.PublishMessage.PayloadAsString;
                Dispositivo? dev = JsonSerializer.Deserialize<Dispositivo>(received_message);                

                Invoke(new MethodInvoker(() => {

                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);
                    row.Cells[0].Value = dev.Nombre;
                    row.Cells[1].Value = dev.Pais;
                    row.Cells[2].Value = dev.Temperatura;
                    row.Cells[3].Value = dev.ConsumoElectrico;
                    row.Cells[4].Value = dev.Estado;
                    row.Cells[5].Value = dev.Fecha;
                    row.Cells[6].Value = dev.Hora;

                    if (dev.Estado == "Precaucion")
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else if (dev.Estado == "Error") {
                        row.DefaultCellStyle.BackColor = Color.IndianRed;
                    }
                    
                    dataGridView1.Rows.Add(row);                    
                }));

            };

            await client.SubscribeAsync("Monitoreo/Dispositivos").ConfigureAwait(false);
        }
    }
}