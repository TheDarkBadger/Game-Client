using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NetworkMessages;

public static class Network
{
    private const string ip = "127.0.0.1";
    private const int port = 3000;
    private static TcpClient client { get; set; } = null;
    private static NetworkStream stream { get; set; } = null;

    public static bool LoggedIn { get; private set; }

    private static BinaryWriter writer = null;
    private static BinaryReader reader = null;


    public static void ConnectToServer()
    {
        if (IsConnected())
        {//Already connected
            return;
        }
        try
        {
            client = new TcpClient();
            client.Connect(IPAddress.Parse(ip), port);
            stream = client.GetStream();
            writer = new BinaryWriter(stream, Encoding.UTF8, true);
            reader = new BinaryReader(stream, Encoding.UTF8, true);

            Task.Run(() => ListenForMessages());
            UIMainMenu.get.UpdateConnectionStatus();

        }
        catch (Exception e)
        {
            DisconnectFromServer();
            Debug.LogError("Could not connect to the server: " + e.Message);
        }

    }

    public static void DisconnectFromServer()
    {
        client?.Close();
        stream?.Close();
        client = null;
        stream = null;
        writer = null;
        reader = null;
        LoggedIn = false;
        UIMainMenu.get.UpdateConnectionStatus();
    }

    public static void Login(string username, string password)
    {
        ConnectToServer();
        Debug.Log("Attempting Login...");
        SendNetworkMessage(MessageType.Login, new string[] { username, password });
    }

    public static void RegisterAccount(string username, string password, string email)
    {
        ConnectToServer();
        Debug.Log("Attempting to Registering account...");
        SendNetworkMessage(MessageType.Register, new string[] { username, password, email });
    }

    /// <summary>
    /// Send a message to the server
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public static void SendNetworkMessage(byte type, string[] toSend)
    {
        if (IsConnected() == false)
            return;
        writer.Write(type);
        for (int i = 0; i < toSend.Length; i++)
            writer.Write(toSend[i]);
        writer.Flush();
    }

    /// <summary>
    /// Listens for messages from the server
    /// </summary>
    public static void ListenForMessages()
    {
        while (IsConnected())
        {
            if (!stream.DataAvailable)
                continue;
            byte messageType = reader.ReadByte();
            try
            {
                switch (messageType)
                {
                    case MessageType.Handshake:
                        if (reader.ReadString() != "RPGGameServer")
                        {//Handshake failed
                            DisconnectFromServer();
                            Debug.LogWarning("Server handshake failed!");
                            break;
                        }
                        SendNetworkMessage(MessageType.Handshake, new string[] { "RPGGameClient" });
                        Debug.Log("Successfully connected!");
                        break;
                    case MessageType.LoginConfirmed:
                        LoggedIn = true;
                        Debug.Log("Successfully logged in!");
                        break;
                    case MessageType.LoginError:
                        LoggedIn = false;
                        Debug.LogWarning($"Error logging in: {reader.ReadString()}");
                        break;
                    case MessageType.RegisterConfirmed:
                        Debug.Log($"Account has been registered");
                        break;
                    case MessageType.RegisterError:
                        Debug.LogWarning($"Error registering an account in: {reader.ReadString()}");
                        break;
                    case MessageType.CharacterList:
                        int characterAmount = int.Parse(reader.ReadString());
                        CharacterBasic[] characters = new CharacterBasic[characterAmount];
                        for (int i = 0; i < characterAmount; i++)
                        {//foreach character
                            characters[i] = new CharacterBasic(reader.ReadString(), reader.ReadString(), int.Parse(reader.ReadString()));
                        }
                        UnityMainThreadDispatcher.Instance().Enqueue(() => UIMainMenu.get.UpdateCharacterList(characters));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR:{messageType}-{e.Message}");
            }
        }
    }

    /// <summary>
    /// Returns true if this client is connected to a server
    /// </summary>
    /// <returns></returns>
    public static bool IsConnected()
    {
        if (client == null)
            return false;
        return true;
    }
}
