﻿// WingsEmu
// 
// Developed by NosWings Team

namespace OpenNos.Core
{
    public class SessionFactory
    {
        #region Instantiation

        private SessionFactory()
        {
        }

        #endregion

        #region Properties

        public static SessionFactory Instance => _instance ?? (_instance = new SessionFactory());

        #endregion

        #region Methods

        public int GenerateSessionId()
        {
            _sessionCounter += 2;
            return _sessionCounter;
        }

        #endregion

        #region Members

        private static SessionFactory _instance;
        private int _sessionCounter;

        #endregion
    }
}