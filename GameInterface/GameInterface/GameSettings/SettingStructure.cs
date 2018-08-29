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
    public class SettingStructure : ViewModels.NotifyDataErrorInfoViewModel
    {
        private ushort limitTime = 5;

        /// <summary>
        /// Limitation Time [Seconds]
        /// </summary>
        public ushort LimitTime {
            get => limitTime;
            set {
                ResetError();
                if (value == 0)
                    AddError("ターン時間は0秒以上でなければなりません．");
                RaisePropertyChanged(ref limitTime, value);
            }
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
            set {
                ResetError();
                if (value < 1)
                    AddError("ターン数は1以上でないといけません");
                RaisePropertyChanged(ref turns, value);
            }
        }

        private byte boardWidth = 10;

        /// <summary>
        /// Width of Board
        /// </summary>
        public byte BoardWidth {
            get => boardWidth;
            set {
                ResetError();
                if (value <= 2)
                    AddError("フィールドの幅は3以上でなければなりません");
                if(value > 12)
                    AddError("フィールドの幅は12以下でなければなりません");
                RaisePropertyChanged(ref boardWidth, value);
            }
        }

        private byte boardHeight = 10;

        /// <summary>
        /// Height of Bord
        /// </summary>
        public byte BoardHeight {
            get => boardHeight;
            set {
                ResetError();
                if(value <= 2)
                    AddError("フィールドの高さは3以上でなければなりません");
                if (value > 12)
                    AddError("フィールドの高さは12以下でなければなりません");
                RaisePropertyChanged(ref boardHeight, value);
            }
        }
    }
}
