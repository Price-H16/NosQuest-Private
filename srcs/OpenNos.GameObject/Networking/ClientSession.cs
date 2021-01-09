// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.Core.Logging;
using OpenNos.Core.Networking;
using OpenNos.GameObject.Commands;
using OpenNos.GameObject.Families;
using OpenNos.GameObject.Maps;
using WingsEmu.Packets;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Networking
{
    public class ClientSession
    {
        private static IGlobalCommandExecutor _commandsExecutor;
        private readonly CommunicationServiceEvents _communicationServiceEvents = new CommunicationServiceEvents();
        private readonly char[] PACKET_SPLITTER = { (char)0xFF };
        private const char COMMAND_PREFIX = '$';

        #region Instantiation

        public static void Initialize(IGlobalCommandExecutor commandExecutor)
        {
            _commandsExecutor = commandExecutor;
        }

        public ClientSession(INetworkSession client)
        {
            // initialize network client
            _client = client;

            // absolutely new instantiated Client has no SessionId
            SessionId = 0;

            // register for NetworkClient events
            // _client.MessageReceived += OnNetworkClientMessageReceived;
            _client.PacketReceived += Client_OnPacketReceived;
        }

        private void Client_OnPacketReceived(object sender, string e)
        {
            try
            {
                HandlePackets(e.Split(PACKET_SPLITTER, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Client_OnPacketReceived : ", ex);
                Disconnect();
            }
        }

        #endregion

        #region Members

        private bool _isWorldServer;
        private Character.Character _character;
        private readonly INetworkSession _client;
        private IDictionary<string, HandlerMethodReference> _handlerMethods;
        private readonly IList<string> _waitForPacketList = new List<string>();

        // Packetwait Packets
        private int? _waitForPacketsAmount;

        // private byte countPacketReceived;

        #endregion

        #region Properties

        public Account Account { get; private set; }

        public Character.Character Character
        {
            get
            {
                if (_character == null || !HasSelectedCharacter)
                {
                    // cant access an
                    Logger.Log.Warn("Uninitialized Character cannot be accessed.");
                }

                return _character;
            }

            private set => _character = value;
        }


        public MapInstance CurrentMapInstance { get; set; }

        public IDictionary<string, HandlerMethodReference> HandlerMethods
        {
            get => _handlerMethods ?? (_handlerMethods = new Dictionary<string, HandlerMethodReference>());

            set => _handlerMethods = value;
        }

        public bool HasCurrentMapInstance => CurrentMapInstance != null;

        public bool HasSelectedCharacter { get; set; }

        public bool HasSession => _client != null;

        public string IpAddress => _client.IpAddress.ToString();

        private bool _isAuthenticated;

        public bool IsAuthenticated
        {
            get
                => _isAuthenticated;
            set
            {
                _isAuthenticated = value;
                _client.SessionId = SessionId;
            }
        }

        public bool IsConnected => _client.IsConnected;

        public bool IsDisposing
        {
            get => _client.IsDisposing;

            set => _client.IsDisposing = value;
        }

        public bool IsOnMap => CurrentMapInstance != null;

        public int LastKeepAliveIdentity { get; set; }

        public DateTime RegisterTime { get; internal set; }

        public int SessionId
        {
            get => _client.SessionId;
            set => _client.SessionId = value;
        }

        #endregion

        #region Methods

        public void ClearLowPriorityQueue()
        {
            // change map instance, clear pending packets or block packets from being processed
        }

        public void Destroy()
        {
            // unregister from events
            _communicationServiceEvents.CharacterConnectedEvent -= OnOtherCharacterConnected;
            _communicationServiceEvents.CharacterDisconnectedEvent -= OnOtherCharacterDisconnected;

            // do everything necessary before removing client, DB save, Whatever
            if (HasSelectedCharacter)
            {
                Character.Dispose();
                if (Character.MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance ||
                    Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    Character.MapInstance.InstanceBag.DeadList.Add(Character.CharacterId);
                    if (Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        Character?.Group?.Characters.ToList().ForEach(s =>
                        {
                            s.SendPacket(s.Character?.Group?.GeneraterRaidmbf(s.CurrentMapInstance));
                            s.SendPacket(s.Character?.Group?.GenerateRdlst());
                        });
                    }
                }

                if (Character?.Miniland != null)
                {
                    ServerManager.Instance.RemoveMapInstance(Character.Miniland.MapInstanceId);
                }

                // TODO Check why ExchangeInfo.TargetCharacterId is null Character.CloseTrade();
                // disconnect client
                CommunicationServiceClient.Instance.DisconnectCharacter(ServerManager.Instance.WorldId,
                    Character.CharacterId);

                // unregister from map if registered
                if (CurrentMapInstance != null)
                {
                    CurrentMapInstance.UnregisterSession(Character.CharacterId);
                    CurrentMapInstance = null;
                    ServerManager.Instance.UnregisterSession(Character.CharacterId);
                }
            }

            if (Account != null)
            {
                CommunicationServiceClient.Instance.DisconnectAccount(Account.AccountId);
            }
        }

        public void Disconnect()
        {
            Character?.AntiBotMessageInterval?.Dispose();
            Character?.AntiBotObservable?.Dispose();
            Character?.SaveObs?.Dispose();
            _client.DisconnectClient();
        }

        public void Initialize(Type packetHandler, bool isWorldServer)
        {
            _isWorldServer = isWorldServer;
            // dynamically create packethandler references
            GenerateHandlerReferences(packetHandler, isWorldServer);
        }

        public void InitializeAccount(Account account, bool crossServer = false)
        {
            Account = account;
            if (crossServer)
            {
                CommunicationServiceClient.Instance.ConnectCrossServerAccount(ServerManager.Instance.WorldId,
                    account.AccountId, SessionId);
            }
            else
            {
                CommunicationServiceClient.Instance.ConnectAccount(ServerManager.Instance.WorldId, account.AccountId,
                    SessionId);
            }

            IsAuthenticated = true;
        }

        //[Obsolete("Primitive string operations will be removed in future, use PacketDefinition SendPacket instead. SendPacket with string parameter should only be used for debugging.")]
        public void SendPacket(string packet)
        {
            if (!IsDisposing)
            {
                _client.SendPacket(packet);
            }
        }

        public void SendPacket(PacketDefinition packet)
        {
            if (!IsDisposing)
            {
                _client.SendPacket(PacketFactory.Serialize(packet));
            }
        }

        public void SendPacketAfter(string packet, int milliseconds)
        {
            if (!IsDisposing)
            {
                Observable.Timer(TimeSpan.FromMilliseconds(milliseconds)).Subscribe(o => { SendPacket(packet); });
            }
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            if (!IsDisposing)
            {
                _client.SendPacketFormat(packet, param);
            }
        }

        //[Obsolete("Primitive string operations will be removed in future, use PacketDefinition SendPacket instead. SendPacket with string parameter should only be used for debugging.")]
        public void SendPackets(IEnumerable<string> packets)
        {
            if (!IsDisposing)
            {
                _client.SendPackets(packets);
            }
        }

        public void SetCharacter(Character.Character character)
        {
            Character = character;

            // register events
            _communicationServiceEvents.CharacterConnectedEvent += OnOtherCharacterConnected;
            _communicationServiceEvents.CharacterDisconnectedEvent += OnOtherCharacterDisconnected;

            HasSelectedCharacter = true;

            // register for servermanager
            ServerManager.Instance.RegisterSession(this);
            Character.SetSession(this);
        }


        private void GenerateHandlerReferences(Type type, bool isWorldServer)
        {
            IEnumerable<Type> handlerTypes = !isWorldServer
                ? type.Assembly.GetTypes()
                    .Where(t => t.Name.Equals("LoginPacketHandler")) // shitty but it works, reflection?
                : type.Assembly.GetTypes().Where(p =>
                {
                    Type interfaceType = type.GetInterfaces().FirstOrDefault();
                    return interfaceType != null && !p.IsInterface && interfaceType.IsAssignableFrom(p);
                });

            // iterate thru each type in the given assembly
            foreach (Type handlerType in handlerTypes)
            {
                var handler = (IPacketHandler)Activator.CreateInstance(handlerType, this);

                // include PacketDefinition
                foreach (MethodInfo methodInfo in handlerType.GetMethods().Where(x =>
                    x.GetCustomAttributes(false).OfType<PacketAttribute>().Any() ||
                    x.GetParameters().FirstOrDefault()?.ParameterType.BaseType == typeof(PacketDefinition)))
                {
                    List<PacketAttribute> packetAttributes =
                        methodInfo.GetCustomAttributes(false).OfType<PacketAttribute>().ToList();

                    // assume PacketDefinition based handler method
                    if (packetAttributes.Count == 0)
                    {
                        var methodReference = new HandlerMethodReference(
                            DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler,
                            methodInfo.GetParameters().FirstOrDefault()?.ParameterType);
                        HandlerMethods.Add(methodReference.Identification, methodReference);
                    }
                    else
                    {
                        // assume string based handler method
                        foreach (PacketAttribute packetAttribute in packetAttributes)
                        {
                            var methodReference = new HandlerMethodReference(
                                DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler,
                                packetAttribute);
                            HandlerMethods.Add(methodReference.Identification, methodReference);
                        }
                    }
                }
            }
        }

        private void ProcessUnAuthedPacket(string sessionPacket)
        {
            string[] sessionParts = sessionPacket.Split(' ');
            if (sessionParts.Length == 0)
            {
                return;
            }

            if (!int.TryParse(sessionParts[0], out int lastka))
            {
                Disconnect();
            }

            LastKeepAliveIdentity = lastka;

            // set the SessionId if Session Packet arrives
            if (sessionParts.Length < 2)
            {
                return;
            }

            if (!int.TryParse(sessionParts[1].Split('\\').FirstOrDefault(), out int sessid))
            {
                return;
            }

            SessionId = sessid;
            Logger.Log.DebugFormat(Language.Instance.GetMessageFromKey("CLIENT_ARRIVED"), SessionId);

            if (!_waitForPacketsAmount.HasValue)
            {
                TriggerHandler("OpenNos.EntryPoint", string.Empty, false);
            }
        }

        /// <summary>
        ///     Handle the packet received by the Client.
        /// </summary>
        private void HandlePackets(IEnumerable<string> packets)
        {
            // determine first packet
            if (_isWorldServer && SessionId == 0)
            {
                ProcessUnAuthedPacket(packets.FirstOrDefault());
                return;
            }

            foreach (string packet in packets)
            {
                string packetstring = packet.Replace('^', ' ');
                string[] packetsplit = packetstring.Split(' ');

                // wtf ???
                if (!_isWorldServer)
                {
                    string packetHeader = packetstring.Split(' ')[0];
                    if (string.IsNullOrWhiteSpace(packetHeader))
                    {
                        Disconnect();
                        return;
                    }

                    // simple messaging
                    if (packetHeader[0] == '/' || packetHeader[0] == ':' || packetHeader[0] == ';')
                    {
                        packetHeader = packetHeader[0].ToString();
                        packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                    }

                    TriggerHandler(packetHeader.Replace("#", ""), packetstring, false);
                    return;
                }

                // keep alive
                string nextKeepAliveRaw = packetsplit[0];
                if (!int.TryParse(nextKeepAliveRaw, out int nextKeepaliveIdentity) && nextKeepaliveIdentity != (LastKeepAliveIdentity + 1))
                {
                    Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CORRUPTED_KEEPALIVE"), IpAddress);
                    _client.DisconnectClient();
                    return;
                }

                if (nextKeepaliveIdentity == 0)
                {
                    if (LastKeepAliveIdentity == ushort.MaxValue)
                    {
                        LastKeepAliveIdentity = nextKeepaliveIdentity;
                    }
                }
                else
                {
                    LastKeepAliveIdentity = nextKeepaliveIdentity;
                }

                if (_waitForPacketsAmount.HasValue)
                {
                    _waitForPacketList.Add(packetstring);
                    string[] packetssplit = packetstring.Split(' ');
                    if (packetssplit.Length > 3 && packetsplit[1] == "DAC")
                    {
                        _waitForPacketList.Add("0 CrossServerAuthenticate");
                    }

                    if (_waitForPacketList.Count != _waitForPacketsAmount)
                    {
                        continue;
                    }

                    _waitForPacketsAmount = null;
                    string queuedPackets = string.Join(" ", _waitForPacketList.ToArray());
                    string header = queuedPackets.Split(' ', '^')[1];
                    TriggerHandler(header, queuedPackets, true);
                    _waitForPacketList.Clear();
                    return;
                }

                if (packetsplit.Length <= 1)
                {
                    continue;
                }

                if (packetsplit[1][0] == COMMAND_PREFIX)
                {
                    // todo Essentials plugin
                    //_commandsExecutor.HandleCommand(packet.Substring(packet.IndexOf(' ') + 1), this);
                    //return;
                }

                if (packetsplit[1].Length >= 1 &&
                    (packetsplit[1][0] == '/' || packetsplit[1][0] == ':' || packetsplit[1][0] == ';'))
                {
                    packetsplit[1] = packetsplit[1][0].ToString();
                    packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                }

                if (packetsplit[1] != "0")
                {
                    TriggerHandler(packetsplit[1].Replace("#", ""), packetstring, false);
                }
            }
        }

        private void OnOtherCharacterConnected(object sender, EventArgs e)
        {
            Tuple<long, string> loggedInCharacter = (Tuple<long, string>)sender;

            if (Character.IsFriendOfCharacter(loggedInCharacter.Item1))
            {
                if (Character != null && Character.CharacterId != loggedInCharacter.Item1)
                {
                    _client.SendPacket(Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_IN"),
                            loggedInCharacter.Item2), 10));
                    _client.SendPacket(Character.GenerateFinfo(loggedInCharacter.Item1, true));
                }
            }

            FamilyCharacter chara =
                Character.Family?.FamilyCharacters.FirstOrDefault(s => s.CharacterId == loggedInCharacter.Item1);
            if (chara != null && loggedInCharacter.Item1 != Character?.CharacterId)
            {
                _client.SendPacket(Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("CHARACTER_FAMILY_LOGGED_IN"),
                        loggedInCharacter.Item2,
                        Language.Instance.GetMessageFromKey(chara.Authority.ToString().ToUpper())), 10));
            }
        }

        private void OnOtherCharacterDisconnected(object sender, EventArgs e)
        {
            (long characterId, string characterName) = (Tuple<long, string>)sender;
            if (!Character.IsFriendOfCharacter(characterId))
            {
                return;
            }

            if (Character == null || Character.CharacterId == characterId)
            {
                return;
            }

            _client.SendPacket(Character.GenerateSay(
                string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_OUT"), characterName),
                10));
            _client.SendPacket(Character.GenerateFinfo(characterId, false));
        }

        private void TriggerHandler(string packetHeader, string packet, bool force)
        {
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            if (IsDisposing)
            {
                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CLIENTSESSION_DISPOSING"), packetHeader);
                return;
            }

            if (!HandlerMethods.TryGetValue(packetHeader, out HandlerMethodReference methodReference))
            {
                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("HANDLER_NOT_FOUND"), packetHeader);
                return;
            }

            if (methodReference.HandlerMethodAttribute != null && !force &&
                methodReference.HandlerMethodAttribute.Amount > 1 && !_waitForPacketsAmount.HasValue)
            {
                // we need to wait for more
                _waitForPacketsAmount = methodReference.HandlerMethodAttribute.Amount;
                _waitForPacketList.Add(packet != string.Empty ? packet : $"1 {packetHeader} ");
                return;
            }

            try
            {
                if (!HasSelectedCharacter &&
                    methodReference.ParentHandler.GetType().Name != "CharacterScreenPacketHandler" &&
                    methodReference.ParentHandler.GetType().Name != "LoginPacketHandler")
                {
                    return;
                }

                // call actual handler method
                if (methodReference.PacketDefinitionParameterType == null)
                {
                    methodReference.HandlerMethod(methodReference.ParentHandler, packet);
                    return;
                }

                //check for the correct authority
                if (IsAuthenticated && (byte)methodReference.Authority > (byte)Account.Authority)
                {
                    return;
                }

                object deserializedPacket = PacketFactory.Deserialize(packet,
                    methodReference.PacketDefinitionParameterType, IsAuthenticated);

                if (deserializedPacket == null && !methodReference.PassNonParseablePacket)
                {
                    Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CORRUPT_PACKET"), packetHeader,
                        packet);
                    return;
                }

                methodReference.HandlerMethod(methodReference.ParentHandler, deserializedPacket);
            }
            catch (Exception ex)
            {
                // disconnect if something unexpected happens
                Logger.Log.Error("Handler Error SessionId: " + SessionId, ex);
                Disconnect();
            }
        }

        #endregion
    }
}