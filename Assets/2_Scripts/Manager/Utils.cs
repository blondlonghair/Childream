using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Utils
{
    public static class Helper
    {
        public static PhotonView PV (this MonoBehaviour comp)
        {
            return comp.GetComponent<PhotonView>();
        }
    }

    public class PRS
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public int index;

        public PRS(Vector3 pos, Quaternion rot, Vector3 scale, int index)
        {
            this.pos = pos;
            this.rot = rot;
            this.scale = scale;
            this.index = index;
        }
    }
}