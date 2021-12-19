After updating Photon Voice 2, you should copy:
Assets/FrostweepGames/_Generic/Tools/CustomMicrophone.cs
to
Assets/Photon/PhotonVoice/PhotonVoiceApi/Platforms/Unity/UnityMicrophone.cs

Then in UnityMicrophone.cs, change the namespace to: Photon.Voice.Unity
and the class name to: UnityMicrophone