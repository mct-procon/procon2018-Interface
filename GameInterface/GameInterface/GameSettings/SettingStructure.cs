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
        private ushort limitTime = 5;

        /// <summary>
        /// Limitation Time [Seconds]
        /// </summary>
        public ushort LimitTime {
            get => limitTime;
            set => RaisePropertyChanged(ref limitTime, value);
        }

        private ushort additionTime = 10;

        /// <summary>
        /// Addition Time [Seconds]
        /// </summary>
        public ushort AdditionTime {
            get => additionTime;
            set => RaisePropertyChanged(ref additionTime, value);
        }

        private ushort port1P = 0;

        /// <summary>
        /// AI TCP/IP Port 1P.
        /// </summary>
        public ushort Port1P {
            get => port1P;
            set => RaisePropertyChanged(ref port1P, value);
        }

        private ushort port2P = 0;

        /// <summary>
        /// AI TCP/IP Port 2P.
        /// </summary>
        public ushort Port2P {
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

        private byte turns = 60;

        /// <summary>
        /// Turn Counts
        /// </summary>
        public byte Turns {
            get => turns;
            set => RaisePropertyChanged(ref turns, value);
        }

        private byte boardWidth = 10;

        /// <summary>
        /// Width of Board
        /// </summary>
        public byte BoardWidth {
            get => boardWidth;
            set => RaisePropertyChanged(ref boardWidth, value);
        }

        private byte boardHeight = 10;

        /// <summary>
        /// Height of Bord
        /// </summary>
        public byte BoardHeight {
            get => boardHeight;
            set => RaisePropertyChanged(ref boardHeight, value);
        }
    }
}
