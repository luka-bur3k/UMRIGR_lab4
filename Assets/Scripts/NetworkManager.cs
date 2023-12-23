using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using AssemblyCSharp.Assets.Scripts;


public abstract class NetworkManager : IDisposable
{
    protected struct Constants
    {
        public const Int32 PORT = 13000;
    }


    protected GameManager gameManager;
    protected TcpClient tcpClient;
    protected UdpClient udpClient;

    protected NetworkManager(GameManager manager)
    {
        gameManager = manager;
    }


    public abstract void StartNetworkManager();


    public void Dispose()
    {
        // provjeriti je li UDP klijent postoji
        // ako UDP klijent postoji zatvoriti ga
        if (udpClient != null)
        {
            udpClient.Dispose();
        }

        // provjeriti je li TCP klijent postoji
        if(tcpClient != null) {
            // ako postoji potrebno je
            // definirati novi responseBuffer
            byte[] responseBuffer = new byte[256];
            // dohvatiti networkStream iz samog tcpClienta
            NetworkStream stream = tcpClient.GetStream();
            // definirati novi ProtokolData objekt i u njemu postaviti poruku na tip EXIT
            // u protokol data postaviti MoveSpace na NULL_SPACE - nije riječ o potezu
            ProtocolData protocolData = new ProtocolData();
            protocolData.messageCode = ProtocolData.MessageCode.EXIT;
            protocolData.space = ProtocolData.MoveSpace.NULL_SPACE;

            //ostaviti ovaj dio koda vezan za kopiranje bytova s odgovarajućim offsetom)
            Buffer.BlockCopy(BitConverter.GetBytes((int)protocolData.messageCode), 0, responseBuffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((int)protocolData.space), 0, responseBuffer, 4, 4);

            // zapisati odgovor iz response buffera u networkStream
            // 
            stream.Write(responseBuffer, 0, 8);
            stream.Read(responseBuffer, 0, 8);
            
            protocolData.messageCode = (ProtocolData.MessageCode)BitConverter.ToInt32(responseBuffer, 0);
            protocolData.space = (ProtocolData.MoveSpace)BitConverter.ToInt32(responseBuffer, 4);

            tcpClient.Dispose();
        }
    }


public void SendMove(int move)
{

        // isključiti u gameManageru mogućnost igranja kroz disable boeard
        gameManager.DisableBoard();
        // definirati novi buffer
        byte[] buffer = new byte[256];
        // dohvatiti networkStream iz samog tcpClienta
        NetworkStream stream = tcpClient.GetStream();
        // definirati novi ProtokolData objekt i u njemu postaviti poruku na tip MOVE
        ProtocolData protocolData = new ProtocolData();
        protocolData.messageCode = ProtocolData.MessageCode.MOVE;
        // u protokol data postaviti MoveSpace na odgovarajuću vrijednost samog poteza
        protocolData.space = (ProtocolData.MoveSpace)move;

        //ostaviti ovaj dio koda vezan za kopiranje bytova s odgovarajućim offsetom)
        Buffer.BlockCopy(BitConverter.GetBytes((int)protocolData.messageCode), 0, buffer, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes((int)protocolData.space), 0, buffer, 4, 4);
        // zapisati buffer u networkStream      
        stream.Write(buffer, 0, 8);     
}


public void Restart()
{
        // definirati novi buffer
        byte[] buffer = new byte[256];
        // dohvatiti networkStream iz samog tcpClienta
        NetworkStream stream = tcpClient.GetStream();
        // definirati novi ProtokolData objekt i u njemu postaviti poruku na tip RESTART
        ProtocolData protocolData = new ProtocolData();
        protocolData.messageCode = ProtocolData.MessageCode.RESTART;
        // u protokol data postaviti MoveSpace na na NULL_SPACE - nije riječ o potezu
        protocolData.space = ProtocolData.MoveSpace.NULL_SPACE;

        Buffer.BlockCopy(BitConverter.GetBytes((int)protocolData.messageCode), 0, buffer, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes((int)protocolData.space), 0, buffer, 4, 4);

        // zapisati buffer u networkStream     
        stream.Write(buffer, 0, 8);
    }

}