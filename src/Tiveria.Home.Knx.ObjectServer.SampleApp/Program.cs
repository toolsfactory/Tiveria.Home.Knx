using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx;
using Tiveria.Home.Knx.Datapoint;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.ObjectServer;


namespace Tiveria.Home.Knx.ObjectServer.SampleApp
{
    class Program
    {
        static byte[] getall = new byte[] { 0x06, 0x20, 0xF0, 0x80, 0x00, 0x16, 0x04, 0x00, 0x00, 0x00, 0xF0, 0xD0 };
        static void Main(string[] args)
        {
            KnxNetIPServiceSerializerFactory.Instance.Register<ObjectServerProtocolServiceSerializer>();
            CreateDatapointMap();
            Int32 port = 12004;
            IPAddress localAddr = IPAddress.Parse("192.168.2.99");
            TcpListener server = new TcpListener(localAddr, port);
            // Start listening for client requests.
            server.Start();
            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                Console.WriteLine($"Remote Endpoint: {client.Client.RemoteEndPoint.ToString()}");

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                int inputlen = 0;

                stream.Write(getall);
                var input = new byte[1024];
                var buffer = new byte[0];
                // Loop to receive all the data sent by the client.
                while ((inputlen = stream.Read(input, 0, input.Length)) != 0)
                {
                    buffer = buffer.Merge(input, inputlen);
                    Console.WriteLine(Environment.NewLine + "-------------------");
                    Console.WriteLine(DateTime.Now.ToLongTimeString());
                    Console.WriteLine($"New Bytes: {inputlen}");
                    Console.WriteLine($"Total Bytes: {buffer.Length}");
                    Console.WriteLine("Buffer: {0}", buffer.ToHex());

                    while (TryParseData(out var req, out var size, buffer))
                    {
                        var header = new ConnectionHeader();
                        var frame = new KnxNetIPFrame(new ObjectServerProtocolService(header, new SetDatapointValueResService(req.StartDataPoint, 0)));
                        var answer = frame.ToBytes();
                        stream.Write(answer);
                        Console.WriteLine("Answer: {0}", answer.ToHex());


                        foreach (var item in req.DataPoints)
                        {
                            Console.WriteLine($"{item.ID}: {item.Value.ToHex()}");
                            if (DatapointsMap.TryGetValue(item.ID, out var dp))
                            {
                                var dpttype = DatapointTypesList.GetTypeById(dp.dptid);
                                if (dpttype != null)
                                {
                                    Console.WriteLine("Decoded: " + dpttype.DecodeString(item.Value, withUnit: true));
                                    if (dp.dptid == "9.001")
                                    {
                                        Console.WriteLine("Invariant: " + dpttype.DecodeString(item.Value, withUnit: true, invariant: true));
                                    }
                                }
                            }
                        }
                        buffer = buffer.Clone(size);
                    }
                }

                // Shutdown and end connection
                client.Close();
            }

            Console.WriteLine("\nHit enter to continue...");
            server.Stop();
            Console.Read();
        }

        static byte[] header = new byte[] { 0x06, 0x20, 0xF0, 0x80 };

        private static bool TryParseData(out SetDatapointValueReqService? result, out int size, Span<byte> data)
        {
            size = 0;
            if (KnxNetIPFrame.TryParse(data.ToArray(), out var frame))
            {
                size = frame.FrameHeader.TotalLength;
                var os = frame.Service as ObjectServerProtocolService;
                if (os != null)
                {
                    result = os.ObjectServerService as SetDatapointValueReqService;
                    return result != null;
                }
            }
            result = null;
            return false;
        }

        private static Dictionary<ushort, HeatingDataPoint> DatapointsMap = new Dictionary<ushort, HeatingDataPoint>();

        private static void CreateDatapointMap()
        {
            DatapointsMap.Add(1, new HeatingDataPoint(1, "hg1", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(2, new HeatingDataPoint(2, "hg1", "Betriebsart", "DPT_HVACContrMode", "20.105", false));
            DatapointsMap.Add(3, new HeatingDataPoint(3, "hg1", "Modulationsgrad  Brennerleistung", "DPT_Scaling", "5.001", false));
            DatapointsMap.Add(4, new HeatingDataPoint(4, "hg1", "Kesseltemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(5, new HeatingDataPoint(5, "hg1", "Sammlertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(6, new HeatingDataPoint(6, "hg1", "Rücklauftemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(7, new HeatingDataPoint(7, "hg1", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(8, new HeatingDataPoint(8, "hg1", "Außentemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(9, new HeatingDataPoint(9, "hg1", "Status Brenner / Flamme", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(10, new HeatingDataPoint(10, "hg1", "Status Heizkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(11, new HeatingDataPoint(11, "hg1", "Status Speicherladepumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(12, new HeatingDataPoint(12, "hg1", "Status 3-Wege-Umschaltventil", "DPT_OpenClose", "1.009", false));
            DatapointsMap.Add(13, new HeatingDataPoint(13, "hg1", "Anlagendruck", "DPT_Value_Pres", "9.006", false));
            DatapointsMap.Add(14, new HeatingDataPoint(14, "hg2", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(15, new HeatingDataPoint(15, "hg2", "Betriebsart", "DPT_HVACContrMode", "20.105", false));
            DatapointsMap.Add(16, new HeatingDataPoint(16, "hg2", "Modulationsgrad / Brennerleistung", "DPT_Scaling", "5.001", false));
            DatapointsMap.Add(17, new HeatingDataPoint(17, "hg2", "Kesseltemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(18, new HeatingDataPoint(18, "hg2", "Sammlertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(19, new HeatingDataPoint(19, "hg2", "Rücklauftemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(20, new HeatingDataPoint(20, "hg2", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(21, new HeatingDataPoint(21, "hg2", "Außentemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(22, new HeatingDataPoint(22, "hg2", "Status Brenner / Flamme", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(23, new HeatingDataPoint(23, "hg2", "Status Heizkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(24, new HeatingDataPoint(24, "hg2", "Status Speicherladepumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(25, new HeatingDataPoint(25, "hg2", "Status 3-Wege-Umschaltventil", "DPT_OpenClose", "1.009", false));
            DatapointsMap.Add(26, new HeatingDataPoint(26, "hg2", "Anlagendruck", "DPT_Value_Pres", "9.006", false));
            DatapointsMap.Add(27, new HeatingDataPoint(27, "hg3", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(28, new HeatingDataPoint(28, "hg3", "Betriebsart", "DPT_HVACContrMode", "20.105", false));
            DatapointsMap.Add(29, new HeatingDataPoint(29, "hg3", "Modulationsgrad / Brennerleistung", "DPT_Scaling", "5.001", false));
            DatapointsMap.Add(30, new HeatingDataPoint(30, "hg3", "Kesseltemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(31, new HeatingDataPoint(31, "hg3", "Sammlertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(32, new HeatingDataPoint(32, "hg3", "Rücklauftemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(33, new HeatingDataPoint(33, "hg3", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(34, new HeatingDataPoint(34, "hg3", "Außentemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(35, new HeatingDataPoint(35, "hg3", "Status Brenner / Flamme", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(36, new HeatingDataPoint(36, "hg3", "Status Heizkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(37, new HeatingDataPoint(37, "hg3", "Status Speicherladepumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(38, new HeatingDataPoint(38, "hg3", "Status 3-Wege-Umschaltventil", "DPT_OpenClose", "1.009", false));
            DatapointsMap.Add(39, new HeatingDataPoint(39, "hg3", "Anlagendruck", "DPT_Value_Pres", "9.006", false));
            DatapointsMap.Add(40, new HeatingDataPoint(40, "hg4", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(41, new HeatingDataPoint(41, "hg4", "Betriebsart", "DPT_HVACContrMode", "20.105", false));
            DatapointsMap.Add(42, new HeatingDataPoint(42, "hg4", "Modulationsgrad / Brennerleistung", "DPT_Scaling", "5.001", false));
            DatapointsMap.Add(43, new HeatingDataPoint(43, "hg4", "Kesseltemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(44, new HeatingDataPoint(44, "hg4", "Sammlertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(45, new HeatingDataPoint(45, "hg4", "Rücklauftemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(46, new HeatingDataPoint(46, "hg4", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(47, new HeatingDataPoint(47, "hg4", "Außentemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(48, new HeatingDataPoint(48, "hg4", "Status Brenner / Flamme", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(49, new HeatingDataPoint(49, "hg4", "Status Heizkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(50, new HeatingDataPoint(50, "hg4", "Status Speicherladepumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(51, new HeatingDataPoint(51, "hg4", "Status 3-Wege-Umschaltventil", "DPT_OpenClose", "1.009", false));
            DatapointsMap.Add(52, new HeatingDataPoint(52, "hg4", "Anlagendruck", "DPT_Value_Pres", "9.006", false));
            DatapointsMap.Add(53, new HeatingDataPoint(53, "sys", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(54, new HeatingDataPoint(54, "sys", "Außentemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(55, new HeatingDataPoint(55, "dh", "Raumtemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(56, new HeatingDataPoint(56, "dh", "Warmwassersolltemperatur", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(57, new HeatingDataPoint(57, "dh", "Programmwahl Heizkreis", "DPT_HVACMode", "20.102", true));
            DatapointsMap.Add(58, new HeatingDataPoint(58, "dh", "Programmwahl Warmwasser", "DPT_DHWMode", "20.103", true));
            DatapointsMap.Add(59, new HeatingDataPoint(59, "dh", "Heizkreis Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(60, new HeatingDataPoint(60, "dh", "Heizkreis Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(61, new HeatingDataPoint(61, "dh", "Heizkreis Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(62, new HeatingDataPoint(62, "dh", "Warmwasser Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(63, new HeatingDataPoint(63, "dh", "Warmwasser Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(64, new HeatingDataPoint(64, "dh", "Warmwasser Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(65, new HeatingDataPoint(65, "dh", "Sollwertkorrektur", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(66, new HeatingDataPoint(66, "dh", "Sparfaktor", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(67, new HeatingDataPoint(67, "mk1", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(68, new HeatingDataPoint(68, "mk1", "Raumtemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(69, new HeatingDataPoint(69, "mk1", "Warmwassersolltemperatur", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(70, new HeatingDataPoint(70, "mk1", "Programmwahl Mischer", "DPT_HVACMode", "20.102", true));
            DatapointsMap.Add(71, new HeatingDataPoint(71, "mk1", "Programmwahl Warmwasser", "DPT_DHWMode", "20.103", true));
            DatapointsMap.Add(72, new HeatingDataPoint(72, "mk1", "Mischer Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(73, new HeatingDataPoint(73, "mk1", "Mischer Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(74, new HeatingDataPoint(74, "mk1", "Mischer Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(75, new HeatingDataPoint(75, "mk1", "Warmwasser Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(76, new HeatingDataPoint(76, "mk1", "Warmwasser Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(77, new HeatingDataPoint(77, "mk1", "Warmwasser Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(78, new HeatingDataPoint(78, "mk1", "Sollwertkorrektur", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(79, new HeatingDataPoint(79, "mk1", "Sparfaktor", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(80, new HeatingDataPoint(80, "mk2", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(81, new HeatingDataPoint(81, "mk2", "Raumtemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(82, new HeatingDataPoint(82, "mk2", "Warmwassersolltemperatur", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(83, new HeatingDataPoint(83, "mk2", "Programmwahl Mischer", "DPT_HVACMode", "20.102", true));
            DatapointsMap.Add(84, new HeatingDataPoint(84, "mk2", "Programmwahl Warmwasser", "DPT_DHWMode", "20.103", true));
            DatapointsMap.Add(85, new HeatingDataPoint(85, "mk2", "Mischer Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(86, new HeatingDataPoint(86, "mk2", "Mischer Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(87, new HeatingDataPoint(87, "mk2", "Mischer Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(88, new HeatingDataPoint(88, "mk2", "Warmwasser Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(89, new HeatingDataPoint(89, "mk2", "Warmwasser Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(90, new HeatingDataPoint(90, "mk2", "Warmwasser Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(91, new HeatingDataPoint(91, "mk2", "Sollwertkorrektur", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(92, new HeatingDataPoint(92, "mk2", "Sparfaktor", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(93, new HeatingDataPoint(93, "mk3", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(94, new HeatingDataPoint(94, "mk3", "Raumtemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(95, new HeatingDataPoint(95, "mk3", "Warmwassersolltemperatur", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(96, new HeatingDataPoint(96, "mk3", "Programmwahl Mischer", "DPT_HVACMode", "20.102", true));
            DatapointsMap.Add(97, new HeatingDataPoint(97, "mk3", "Programmwahl Warmwasser", "DPT_DHWMode", "20.103", true));
            DatapointsMap.Add(98, new HeatingDataPoint(98, "mk3", "Mischer Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(99, new HeatingDataPoint(99, "mk3", "Mischer Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(100, new HeatingDataPoint(100, "mk3", "Mischer Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(101, new HeatingDataPoint(101, "mk3", "Warmwasser Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(102, new HeatingDataPoint(102, "mk3", "Warmwasser Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(103, new HeatingDataPoint(103, "mk3", "Warmwasser Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(104, new HeatingDataPoint(104, "mk3", "Sollwertkorrektur", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(105, new HeatingDataPoint(105, "mk3", "Sparfaktor", "DPT_Tempd", "9.002", true));
            DatapointsMap.Add(106, new HeatingDataPoint(106, "km", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(107, new HeatingDataPoint(107, "km", "Sammlertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(108, new HeatingDataPoint(108, "km", "Gesamtmodulationsgrad", "DPT_Scaling", "5.001", false));
            DatapointsMap.Add(109, new HeatingDataPoint(109, "km", "Vorlauftemperatur Mischerkreis", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(110, new HeatingDataPoint(110, "km", "Status Mischerkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(111, new HeatingDataPoint(111, "km", "Status Ausgang A1", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(112, new HeatingDataPoint(112, "km", "Eingang E1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(113, new HeatingDataPoint(113, "km", "Eingang E2", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(114, new HeatingDataPoint(114, "mm1", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(115, new HeatingDataPoint(115, "mm1", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(116, new HeatingDataPoint(116, "mm1", "Vorlauftemperatur Mischerkreis", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(117, new HeatingDataPoint(117, "mm1", "Status Mischerkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(118, new HeatingDataPoint(118, "mm1", "Status Ausgang A1", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(119, new HeatingDataPoint(119, "mm1", "Eingang E1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(120, new HeatingDataPoint(120, "mm1", "Eingang E2", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(121, new HeatingDataPoint(121, "mm2", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(122, new HeatingDataPoint(122, "mm2", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(123, new HeatingDataPoint(123, "mm2", "Vorlauftemperatur Mischerkreis", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(124, new HeatingDataPoint(124, "mm2", "Status Mischerkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(125, new HeatingDataPoint(125, "mm2", "Status Ausgang A1", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(126, new HeatingDataPoint(126, "mm2", "Eingang E1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(127, new HeatingDataPoint(127, "mm2", "Eingang E2", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(128, new HeatingDataPoint(128, "mm3", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(129, new HeatingDataPoint(129, "mm3", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(130, new HeatingDataPoint(130, "mm3", "Vorlauftemperatur Mischerkreis", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(131, new HeatingDataPoint(131, "mm3", "Status Mischerkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(132, new HeatingDataPoint(132, "mm3", "Status Ausgang A1", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(133, new HeatingDataPoint(133, "mm3", "Eingang E1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(134, new HeatingDataPoint(134, "mm3", "Eingang E2", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(135, new HeatingDataPoint(135, "sol", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(136, new HeatingDataPoint(136, "sol", "Warmwassertemperatur Solar 1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(137, new HeatingDataPoint(137, "sol", "Temperatur Kollektor 1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(138, new HeatingDataPoint(138, "sol", "Eingang E1", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(139, new HeatingDataPoint(139, "sol", "Eingang E2 (Durchfluss)", "DPT_Value_Volume_Flow", "9.025", false));
            DatapointsMap.Add(140, new HeatingDataPoint(140, "sol", "Eingang E3", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(141, new HeatingDataPoint(141, "sol", "Status Solarkreispumpe SKP1", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(142, new HeatingDataPoint(142, "sol", "Status Ausgang A1", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(143, new HeatingDataPoint(143, "sol", "Status Ausgang A2", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(144, new HeatingDataPoint(144, "sol", "Status Ausgang A3", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(145, new HeatingDataPoint(145, "sol", "Status Ausgang A4", "DPT_Enable", "1.003", false));
            DatapointsMap.Add(146, new HeatingDataPoint(146, "sol", "Durchfluss", "DPT_Value_Volume_Flow", "9.025", false));
            DatapointsMap.Add(147, new HeatingDataPoint(147, "sol", "aktuelle Leistung", "DPT_Power", "9.024", false));
            DatapointsMap.Add(148, new HeatingDataPoint(148, "cwl", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(149, new HeatingDataPoint(149, "cwl", "Programm CWL", "DPT_DHWMode", "20.103", true));
            DatapointsMap.Add(150, new HeatingDataPoint(150, "cwl", "Zeitprogramm 1", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(151, new HeatingDataPoint(151, "cwl", "Zeitprogramm 2", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(152, new HeatingDataPoint(152, "cwl", "Zeitprogramm 3", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(153, new HeatingDataPoint(153, "cwl", "Zeitweise Intensivlüftung AN/AUS", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(154, new HeatingDataPoint(154, "cwl", "Zeitweise Intensivlüftung Startdatum", "DPT_Date", "11.001", true));
            DatapointsMap.Add(155, new HeatingDataPoint(155, "cwl", "Zeitweise Intensivlüftung Enddatum", "DPT_Date", "11.001", true));
            DatapointsMap.Add(156, new HeatingDataPoint(156, "cwl", "Zeitweise Intensivlüftung Startzeit", "DPT_TimeOfDay", "10.001", true));
            DatapointsMap.Add(157, new HeatingDataPoint(157, "cwl", "Zeitweise Intensivlüftung Endzeit", "DPT_TimeOfDay", "10.001", true));
            DatapointsMap.Add(158, new HeatingDataPoint(158, "cwl", "Zeitweiser Feuchteschutz AN/AUS", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(159, new HeatingDataPoint(159, "cwl", "Zeitweiser Feuchteschutz Startdatum", "DPT_Date", "11.001", true));
            DatapointsMap.Add(160, new HeatingDataPoint(160, "cwl", "Zeitweiser Feuchteschutz Enddatum", "DPT_Date", "11.001", true));
            DatapointsMap.Add(161, new HeatingDataPoint(161, "cwl", "Zeitweiser Feuchteschutz Startzeit", "DPT_TimeOfDay", "10.001", true));
            DatapointsMap.Add(162, new HeatingDataPoint(162, "cwl", "Zeitweiser Feuchteschutz Endzeit", "DPT_TimeOfDay", "10.001", true));
            DatapointsMap.Add(163, new HeatingDataPoint(163, "cwl", "Lüftungsstufe", "DPT_Scaling", "5.001", false));
            DatapointsMap.Add(164, new HeatingDataPoint(164, "cwl", "Ablufttemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(165, new HeatingDataPoint(165, "cwl", "Frischlufttemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(166, new HeatingDataPoint(166, "cwl", "Luftdurchsatz Zuluft", "DPT_FlowRate_m3/h", "13.002", false));
            DatapointsMap.Add(167, new HeatingDataPoint(167, "cwl", "Luftdurchsatz Abluft", "DPT_FlowRate_m3/h", "13.002", false));
            DatapointsMap.Add(168, new HeatingDataPoint(168, "cwl", "Bypass Initialisierung", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(169, new HeatingDataPoint(169, "cwl", "Bypass öffnet/offen", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(170, new HeatingDataPoint(170, "cwl", "Bypass schließt/geschlossen", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(171, new HeatingDataPoint(171, "cwl", "Bypass Fehler", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(172, new HeatingDataPoint(172, "cwl", "Frost Status: Initialisierung/Warte", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(173, new HeatingDataPoint(173, "cwl", "Frost Status: Kein Frost", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(174, new HeatingDataPoint(174, "cwl", "Frost Status: Vorwärmer", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(175, new HeatingDataPoint(175, "cwl", "Frost Status: Fehler/Unausgeglichen", "DPT_Bool", "1.002", false));
            DatapointsMap.Add(176, new HeatingDataPoint(176, "hg1", "Störung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(177, new HeatingDataPoint(177, "hg1", "Betriebsart", "DPT_HVACContrMode", "20.105", false));
            DatapointsMap.Add(178, new HeatingDataPoint(178, "hg1", "Heizleistung", "DPT_Power", "9.024", false));
            DatapointsMap.Add(179, new HeatingDataPoint(179, "hg1", "Kühlleistung", "DPT_Power", "9.024", false));
            DatapointsMap.Add(180, new HeatingDataPoint(180, "hg1", "Kesseltemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(181, new HeatingDataPoint(181, "hg1", "Sammlertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(182, new HeatingDataPoint(182, "hg1", "Rücklauftemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(183, new HeatingDataPoint(183, "hg1", "Warmwassertemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(184, new HeatingDataPoint(184, "hg1", "Außentemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(185, new HeatingDataPoint(185, "hg1", "Status Heizkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(186, new HeatingDataPoint(186, "hg1", "Status Zubringer-/Heizkreispumpe", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(187, new HeatingDataPoint(187, "hg1", "Status 3-Wege-Umschaltventil HZ/WW", "DPT_OpenClose", "1.009", false));
            DatapointsMap.Add(188, new HeatingDataPoint(188, "hg1", "Status 3-Wege-Umschaltventil HZ/K", "DPT_OpenClose", "1.009", false));
            DatapointsMap.Add(189, new HeatingDataPoint(189, "hg1", "Status E-Heizung", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(190, new HeatingDataPoint(190, "hg1", "Anlagendruck", "DPT_Value_Pres", "9.006", false));
            DatapointsMap.Add(191, new HeatingDataPoint(191, "hg1", "Leistungsaufnahme", "DPT_Power", "9.024", false));
            DatapointsMap.Add(192, new HeatingDataPoint(192, "cwl", "Filterwarnung aktiv", "DPT_Switch", "1.001", false));
            DatapointsMap.Add(193, new HeatingDataPoint(193, "cwl", "Filterwarnung zurücksetzen", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(194, new HeatingDataPoint(194, "sys", "1xWarmwasserladung", "DPT_Switch", "1.001", true));
            DatapointsMap.Add(195, new HeatingDataPoint(195, "sol", "Tagesertrag", "DPT_ActiveEnergy", "13.010", false));
            DatapointsMap.Add(196, new HeatingDataPoint(196, "sol", "Gesamtertrag", "DPT_ActiveEnergy_kWh", "13.013", false));
            DatapointsMap.Add(197, new HeatingDataPoint(197, "hg1", "Abgastemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(198, new HeatingDataPoint(198, "hg1", "Leistungsvorgabe", "DPT_Scaling", "5.001", true));
            DatapointsMap.Add(199, new HeatingDataPoint(199, "hg1", "Kesselsolltemperaturvorgabe", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(200, new HeatingDataPoint(200, "hg2", "Abgastemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(201, new HeatingDataPoint(201, "hg2", "Leistungsvorgabe", "DPT_Scaling", "5.001", true));
            DatapointsMap.Add(202, new HeatingDataPoint(202, "hg2", "Kesselsolltemperaturvorgabe", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(203, new HeatingDataPoint(203, "hg3", "Abgastemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(204, new HeatingDataPoint(204, "hg3", "Leistungsvorgabe", "DPT_Scaling", "5.001", true));
            DatapointsMap.Add(205, new HeatingDataPoint(205, "hg3", "Kesselsolltemperaturvorgabe", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(206, new HeatingDataPoint(206, "hg4", "Abgastemperatur", "DPT_Value_Temp", "9.001", false));
            DatapointsMap.Add(207, new HeatingDataPoint(207, "hg4", "Leistungsvorgabe", "DPT_Scaling", "5.001", true));
            DatapointsMap.Add(208, new HeatingDataPoint(208, "hg4", "Kesselsolltemperaturvorgabe", "DPT_Value_Temp", "9.001", true));
            DatapointsMap.Add(209, new HeatingDataPoint(209, "km", "Gesamtmodulationsgradvorgabe", "DPT_Scaling", "5.001", true));
            DatapointsMap.Add(210, new HeatingDataPoint(210, "km", "Sammlersolltemeraturvorgabe", "DPT_Value_Temp", "9.001", true));

        }

        public record HeatingDataPoint(ushort id, string device, string name, string dptname, string dptid, bool writeable);

    }
}