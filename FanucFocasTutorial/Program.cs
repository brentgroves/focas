using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FanucFocasTutorial
{
    class Program
    {
        // short _ret = 0;  // Stores our return value
        private IManagedMqttClient managedMqttClientPublisher;
        string fanucIP = "10.1.90.4";
        string mqttServer = "10.1.1.83";
        ushort fanucHandle = 0;
        string partCounter = "";
        /// <summary>
        /// The managed publisher client.
        /// </summary>
        //  static IManagedMqttClient managedMqttClientPublisher;


        static void Main(string[] args)
        {
            // ushort handle = 0;
            Program myObj = new Program();
            // myObj.PublishMQTT();
            // return;
            // If we specified an ip address, get it from the args
            if (args.Length > 0)
                myObj.fanucIP = args[0];

            // myObj.GetCounter(526);//cnc362 10.1.90.4
            //Sample(511);//cnc422 10.1.90.5
            //Sample(501);//cnc422 10.1.90.2

            // Free the Focas handle
            // Focas1.cnc_freelibhndl(_handle);
            myObj.GetCounter(526);
            myObj.test();


        }

        /* number is variable number to be read. */
        short GetCounter(short number)
        {
            short ret = -1;
            try
            {
                ret = Focas1.cnc_allclibhndl3(this.fanucIP, 8193, 6, out this.fanucHandle);
                if (ret == Focas1.EW_OK)
                {
                    Console.WriteLine("We are connected!");
                }
                else
                {
                    // Console.WriteLine("There was an error connecting. Return value: " + _ret);
                    throw new Exception("There was an error connecting. Return value: " + ret);
                }

                Focas1.ODBM macro = new Focas1.ODBM();
                string strVal;
                ret = Focas1.cnc_rdmacro(this.fanucHandle, number, 10, macro);
                if (ret == Focas1.EW_OK)
                {
                    strVal = string.Format("{0:d9}", Math.Abs(macro.mcr_val));
                    if (0 < macro.dec_val) strVal = strVal.Insert(9 - macro.dec_val, ".");
                    if (macro.mcr_val < 0) strVal = "-" + strVal;
                    Console.WriteLine("{0}", strVal);
                    this.partCounter = strVal;
                }
                else
                {
                    throw new Exception("There was an error reading the macro variable. Return value: " + ret);
                    // Console.WriteLine("**********");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception => {0}", e);
            }
            finally
            {
                // Free the Focas handle
                Focas1.cnc_freelibhndl(this.fanucHandle);
            }
            return ret;

        }
        async void test()
        {
            var mqttFactory = new MqttFactory();
            var tlsOptions = new MqttClientTlsOptions
            {
                UseTls = false,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                AllowUntrustedCertificates = true
            };
            var options = new MqttClientOptions
            {
                ClientId = "ClientPublisher",
                ProtocolVersion = MqttProtocolVersion.V311,
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = "10.1.1.83",
                    Port = 1883,
                    TlsOptions = tlsOptions
                }
            };
            if (options.ChannelOptions == null)
            {
                throw new InvalidOperationException();
            }
            options.Credentials = new MqttClientCredentials
            {
                Username = "username",
                Password = Encoding.UTF8.GetBytes("password")
            };
            options.CleanSession = true;
            options.KeepAlivePeriod = TimeSpan.FromSeconds(5);
            this.managedMqttClientPublisher = mqttFactory.CreateManagedMqttClient();
            this.managedMqttClientPublisher.UseApplicationMessageReceivedHandler(this.HandleReceivedApplicationMessage);
            this.managedMqttClientPublisher.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnPublisherConnected);
            this.managedMqttClientPublisher.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnPublisherDisconnected);
            await this.managedMqttClientPublisher.StartAsync(
                new ManagedMqttClientOptions
                {
                    ClientOptions = options
                });
            Console.ReadLine();
            try
            {
                var payload = Encoding.UTF8.GetBytes(this.partCounter);
                var message = new MqttApplicationMessageBuilder().WithTopic("presence").WithPayload(payload).WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag().Build();

                if (this.managedMqttClientPublisher != null)
                {
                    await this.managedMqttClientPublisher.PublishAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();

            if (this.managedMqttClientPublisher == null)
            {
                return;
            }

            await this.managedMqttClientPublisher.StopAsync();
            this.managedMqttClientPublisher = null;

        }

        /// <summary>
        /// Handles the publisher connected event.
        /// </summary>
        /// <param name="x">The MQTT client connected event args.</param>
        private static void OnPublisherConnected(MqttClientConnectedEventArgs x)
        {
            Console.WriteLine("Publisher Connected");
        }

        /// <summary>
        /// Handles the publisher disconnected event.
        /// </summary>
        /// <param name="x">The MQTT client disconnected event args.</param>
        private static void OnPublisherDisconnected(MqttClientDisconnectedEventArgs x)
        {
            Console.WriteLine("Publisher Disconnected");
        }

        /// <summary>
        /// Handles the received application message event.
        /// </summary>
        /// <param name="x">The MQTT application message received event args.</param>
        private void HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs x)
        {
            var item = $"Timestamp: {DateTime.Now:O} | Topic: {x.ApplicationMessage.Topic} | Payload: {x.ApplicationMessage.ConvertPayloadToString()} | QoS: {x.ApplicationMessage.QualityOfServiceLevel}";
            // this.BeginInvoke((MethodInvoker)delegate { this.TextBoxSubscriber.Text = item + Environment.NewLine + this.TextBoxSubscriber.Text; });
        }
    }

}
