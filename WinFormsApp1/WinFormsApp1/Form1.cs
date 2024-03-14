using HiveMQtt.Client;
using HiveMQtt.Client.Results;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using System.Text.Json;

namespace WinFormsApp1
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

        private async void ConexionBroker() {
            var options = new HiveMQClientOptions();
            options.Host = "4836bdded3434bc1b55eff77116d15f1.s1.eu.hivemq.cloud";
            options.Port = 8883;
            options.UserName = "Cliente1";
            options.Password = "Cliente1$";

            client = new HiveMQClient(options);

            ConnectResult connectResult = await client.ConnectAsync().ConfigureAwait(false);

            if (connectResult.ReasonCode == ConnAckReasonCode.Success)
            {
                
            }
            else
            {
                MessageBox.Show("Conexión no realizada con el Broker", "Conexión Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }

        //Inicia envio de datos
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;            

            //Construccion dispotivo            
            dev.Nombre = textBox1.Text;
            dev.Pais = textBox2.Text;

            label4.Text = dev.Nombre;
            label5.Text = dev.Pais;
            //Ejecutar brocker
            backgroundWorker1.RunWorkerAsync();

            label3.Text = "Conectado " + DateTime.Now;
            label3.ForeColor = System.Drawing.Color.Green;
            pictureBox1.Image = Properties.Resources.device___success;            
        }


        //Se cancela el envio de datos
        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            backgroundWorker1.CancelAsync();
            label3.Text = "Desconectado";
            label3.ForeColor = System.Drawing.Color.Red;
            pictureBox1.Image = Properties.Resources.device___danger;
        }

        #region Generacion de datos aleatorios
        public double GenerarTemperatura() {
            Random random = new Random();
            double number = random.Next(20, 30) + random.NextDouble();
            return Math.Round(number, 2);
        }

        public double GenerarElectricidad()
        {
            Random random = new Random();
            double number = random.Next(1000, 1200) + random.NextDouble();
            return Math.Round(number, 2);
        }

        public string GenerarEstado() {
            Random random = new Random();
            double number = random.Next(1, 4);
            string estado = "";

            switch (number) {
                case 1:
                    estado = "Correcto";
                    break;
                case 2:
                    estado = "Precaucion";
                    break;
                case 3:
                    estado = "Error";
                    break;
                case 4:
                    estado = "Correcto";
                    break;

                default:
                    estado = "No identificado";
                    break;
            }

            return estado;
        }
        #endregion


        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            MessageBox.Show("Conexión realizada con exisito con el Broker", "Conexión Existosa", MessageBoxButtons.OK, MessageBoxIcon.Information);                        


            //Enviar mensaje
            while (!backgroundWorker1.CancellationPending)
            {                

                Invoke(new MethodInvoker(() => {
                    label6.Text = "Preparando informacion...";
                    label6.ForeColor = Color.Yellow;
                }));

                Thread.Sleep(5000);

                dev.Temperatura = GenerarTemperatura().ToString();
                dev.ConsumoElectrico = GenerarElectricidad().ToString(); 
                dev.Estado = GenerarEstado();
                dev.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                dev.Hora = DateTime.Now.ToString("HH:mm:ss");

                var json = JsonSerializer.Serialize<Dispositivo>(dev);
                
                client.PublishAsync("Monitoreo/Dispositivos", json).ConfigureAwait(false);
                
                Invoke(new MethodInvoker(() => {
                    label6.Text = "Información enviada... " + DateTime.Now;
                    label6.ForeColor = Color.Green;
                }));

                Thread.Sleep(5000);

            }

        }
    }
}