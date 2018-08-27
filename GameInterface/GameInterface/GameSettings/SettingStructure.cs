using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface.GameSettings
{
    /// <summary>
    /// Game Settings Structure
    /// </summary>
    internal class SettingStructure : ViewModelBase
    {
        private int limitTime = 5;

        /// <summary>
        /// Limitation Time [Seconds]
        /// </summary>
        internal int LimitTime {
            get => limitTime;
            set => RaisePropertyChanged(ref limitTime, value);
        }

        private int port1P = 0;

        /// <summary>
        /// AI TCP/IP Port 1P.
        /// </summary>
        internal int Port1P {
            get => port1P;
            set => RaisePropertyChanged(ref port1P, value);
        }

        private int port2P = 0;

        /// <summary>
        /// AI TCP/IP Port 2P.
        /// </summary>
        internal int Port2P {
            get => port2P;
            set => RaisePropertyChanged(ref port2P, value);
        }

        /// <summary>
        /// Whether 1P is a user.
        /// </summary>
        internal bool IsUser1P => port1P == 0;

        /// <summary>
        /// Whether 2P is a user.
        /// </summary>
        internal bool IsUser2P => port2P == 0;

        private int turns = 60;

        /// <summary>
        /// Turn Counts
        /// </summary>
        internal int Turns {
            get => turns;
            set => RaisePropertyChanged(ref turns, value);
        }
    }
}
