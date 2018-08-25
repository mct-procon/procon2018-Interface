using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    public class Cell : ViewModelsBase
    {
        public enum AreaState { FREE, AREA_1P, AREA_2P };
        private int score;
        public int Score
        {
            get => score; 
            set
            {
                score = value;
                RaisePropertyChanged("Score");
            }
        }
        private AreaState areaState;
        public AreaState AreaState_
        {
            get => areaState; 
            set
            {
                areaState = value;
                RaisePropertyChanged("AreaState_");
            }
        }
        public Cell() { }
        public Cell(int _score)
        {
            Score = _score;
        }

    }
}
