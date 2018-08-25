using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    public class Cell : ViewModelBase
    {
        public enum AreaState { FREE, AREA_1P, AREA_2P };
        private int score;
        public int Score
        {
            get => score; 
            set => RaisePropertyChanged(ref score, value);
        }
        private AreaState areaState = AreaState.FREE;
        public AreaState AreaState_
        {
            get => areaState;
            set => RaisePropertyChanged(ref areaState, value);
        }
        public Cell() { }
        public Cell(int _score)
        {
            Score = _score;
        }
    }
}
