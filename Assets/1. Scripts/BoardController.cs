using System.Collections; using System.Collections.Generic; using UnityEngine; using TMPro; using UnityEngine.UI; using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<GameObject> _plucks = new();
    [SerializeField] List<GameObject> _hiddenConnectors = new();
    [SerializeField] List<Image> _lives = new();
    List<(GameObject connector, GameObject pluck)> _activeConnectors = new();
    List<(GameObject pluck, int[] labelId)> _assignedPlucks = new();

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Image _timerBar;
    [SerializeField] TextMeshProUGUI _panelText;
    [SerializeField] TextMeshProUGUI _dialogueText;
    [SerializeField] GameObject _manualCanvas;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _victoryScreen;
    [SerializeField] AudioClip _phoneRingingAudio;
    [SerializeField] AudioClip _personSpeakingAudio;
    [SerializeField] AudioClip _correctAudio;
    [SerializeField] AudioClip _incorrectAudio;
    [SerializeField] AudioSource _introAudioSource;

    [SerializeField] int _health = 3;
    [SerializeField] int _timer = 600;
    [SerializeField] int _roundsToWin = 5;
    [SerializeField] List<(GameObject pluck, int[] labelId)> _currentCallers;
    bool _enableBoard = false;
    Coroutine _countDown;
    enum City {Madrid, Lebrija, Seville, Cadiz, Toledo}  enum Home {Duplex, Flat, Bungalow, Mansion}  enum Contract {Rented, Owned}  enum Job {Mechanic = 1, Teacher = 2, Firefighter = 3, Policeperson = 4, Cashier = 5, Baker = 6}  enum MaritalStatus {Single = 0, Engaged = 1, Divorced = 7, Married = 8, Widowed = 9}  enum SocialClass {Lower, Middle, Upper}
    
    private void OnEnable() { Application.runInBackground = true; SetLabels(); NextCaller(); }

    public void NewGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    public void OnSwitchClick(GameObject button) {  if (!_enableBoard) return;

                                                    if (_activeConnectors.Count < 2) {  _hiddenConnectors[0].transform.position = button.transform.position;
                                                                                        _hiddenConnectors[0].SetActive(true);
                                                                                        _activeConnectors.Add(new(_hiddenConnectors[0], button.transform.parent.gameObject));
                                                                                        _hiddenConnectors.RemoveAt(0); }

                                                    if (_activeConnectors.Count == 2) { _lineRenderer.enabled = true;
                                                                                        _lineRenderer.SetPosition(0, new(_activeConnectors[0].connector.transform.position.x, _activeConnectors[0].connector.transform.position.y, _activeConnectors[0].connector.transform.position.z - 1));
                                                                                        _lineRenderer.SetPosition(1, new(_activeConnectors[1].connector.transform.position.x, _activeConnectors[1].connector.transform.position.y, _activeConnectors[1].connector.transform.position.z - 1)); }}

    public void RemoveConnector(GameObject go) {    (GameObject connector, GameObject pluck) tuple = _activeConnectors.Find(x => x.connector == (go));
                                                    if (tuple != (null, null)) { _activeConnectors.Remove(tuple); _hiddenConnectors.Add(go); go.SetActive(false); _lineRenderer.enabled = false; }}

    public void NextCaller() 
    {  
        _panelText.text = ""; _dialogueText.text = "";
        for (int i = 0; i < 3 - _health; i++) { _lives[i].color = Color.black; }
        if (_activeConnectors.Count == 2) { RemoveConnector(_activeConnectors[0].connector); RemoveConnector(_activeConnectors[0].connector); }
        if (_health <= 0) { _gameOverScreen.SetActive(true); gameObject.SetActive(false); _manualCanvas.SetActive(false); return; } else if (_roundsToWin <= 0) { _victoryScreen.SetActive(true); gameObject.SetActive(false); return; }
        
        foreach(GameObject pluck in _plucks)
        {
            pluck.transform.GetChild(2).gameObject.SetActive(false);
        }

        StartCoroutine(TransitionToNextCaller()); 
    }

    public void ConfirmSelection() {    if (_activeConnectors.Count < 2) return; GetComponent<AudioSource>().Stop(); _introAudioSource.Stop();
        if (!_currentCallers.Exists(x => x.pluck == _activeConnectors[0].pluck) || !_currentCallers.Exists(x => x.pluck == _activeConnectors[1].pluck)) { _health--; GetComponent<AudioSource>().PlayOneShot(_incorrectAudio, 1.3f); } else GetComponent<AudioSource>().PlayOneShot(_correctAudio, 2f);
                                        StopCoroutine(_countDown); NextCaller(); }

    private void SetLabels() { foreach(GameObject pluck in _plucks) {   string label; int[] labelId = CreateLabelId(label = Random.Range(0, 999999).ToString("D6"));
                                                                        while(_assignedPlucks.Exists(x => x.labelId == labelId)) labelId = CreateLabelId(label = Random.Range(0, 999999).ToString("D6"));
                                                                        pluck.GetComponentInChildren<TextMeshProUGUI>().text = label; _assignedPlucks.Add((pluck, labelId));} }

    private int[] CreateLabelId(string label) { int city = 0; int home = 0; int contract = 0; int job = 0; int status = -1; int _class = 0;
                                                int digits12 = int.Parse(label.Substring(0, 2)); int digit3 = int.Parse(label.Substring(2, 1)); int digit4 = int.Parse(label.Substring(3, 1)); int digit3Parity = digit3 % 2; int digit4Parity = digit4 % 2; int digit6 = int.Parse(label.Substring(5, 1)); int digit5 = int.Parse(label.Substring(4, 1)); int digit1 = int.Parse(label.Substring(0, 1)); int digit2 = int.Parse(label.Substring(1, 1)); int evenAmount = 0; int primeAmount = 0; int sub = digit3 - digit5; int sum = digit4 + digit6;
                                                if      ((digits12 >= 00 && digits12 <= 08) || (digits12 >= 55 && digits12 <= 64) || digits12 == 93)                     city = (int)City.Madrid;
                                                else if ((digits12 >= 31 && digits12 <= 40) || (digits12 >= 71 && digits12 <= 74) || (digits12 >= 94 && digits12 <= 99)) city = (int)City.Lebrija;
                                                else if ((digits12 >= 16 && digits12 <= 22) || (digits12 >= 41 && digits12 <= 46) || (digits12 >= 82 && digits12 <= 88)) city = (int)City.Seville;
                                                else if ((digits12 >= 09 && digits12 <= 15) || (digits12 >= 65 && digits12 <= 70) || (digits12 >= 75 && digits12 <= 81)) city = (int)City.Cadiz;
                                                else if ((digits12 >= 23 && digits12 <= 30) || (digits12 >= 47 && digits12 <= 54) || (digits12 >= 89 && digits12 <= 92)) city = (int)City.Toledo;

                                                bool option1 = (city == (int)City.Seville || city == (int)City.Madrid); if (city == (int)City.Toledo)                 home = (int)Home.Flat;
                                                else if ((option1 && digit3Parity == 0 && digit4Parity != 0) || (!option1 && digit3Parity == 0 && digit4Parity == 0)) home = (int)Home.Duplex;
                                                else if ((option1 && digit3Parity != 0 && digit4Parity == 0) || (!option1 && digit3Parity != 0 && digit4Parity != 0)) home = (int)Home.Flat;
                                                else if ((option1 && digit3Parity == 0 && digit4Parity == 0) || (!option1 && digit3Parity != 0 && digit4Parity == 0)) home = (int)Home.Bungalow;
                                                else if ((option1 && digit3Parity != 0 && digit4Parity != 0) || (!option1 && digit3Parity == 0 && digit4Parity != 0)) home = (int)Home.Mansion;

                                                if (!CheckIfPrime(digit6)){ if      (city == (int)City.Lebrija || city == (int)City.Toledo)                                   contract = (int)Contract.Rented;
                                                                            else if (city == (int)City.Madrid && (home == (int)Home.Flat || home == (int)Home.Mansion))       contract = (int)Contract.Owned;
                                                                            else if (city == (int)City.Madrid && (home != (int)Home.Flat && home != (int)Home.Mansion))       contract = (int)Contract.Rented;
                                                                            else if (city == (int)City.Seville && home != (int)Home.Bungalow)                                   contract = (int)Contract.Owned;
                                                                            else if (city == (int)City.Seville && home == (int)Home.Bungalow)                                   contract = (int)Contract.Rented;
                                                                            else                                                                                              contract = (int)Contract.Owned;}
                                                else                      { if      (home == (int)Home.Duplex)                                                                contract = (int)Contract.Owned;
                                                                            else if (home != (int)Home.Mansion && (city == (int)City.Madrid || city == (int)City.Seville))   contract = (int)Contract.Rented;
                                                                            else if (city == (int)City.Lebrija || city == (int)City.Cadiz || city == (int)City.Toledo)        contract = (int)Contract.Owned;
                                                                            else                                                                                              contract = (int)Contract.Rented;}

                                                if      (sum == 0 || sum == 3 || sum == 6 || sum == 9  || sum == 12 || sum == 15 || sum == 18)  job = sub < 0 ? (int)Job.Mechanic    : (int)Job.Baker;
                                                else if (sum == 2 || sum == 5 || sum == 8 || sum == 11 || sum == 14 || sum == 17)              job = sub < 0 ? (int)Job.Teacher      : (int)Job.Policeperson;
                                                else if (sum == 1 || sum == 4 || sum == 7 || sum == 10 || sum == 13 || sum == 16)              job = sub < 0 ? (int)Job.Firefighter  : (int)Job.Cashier;

                                                foreach (char digit in label) { if ((digit - '0') % 2 == 0) evenAmount++; if (CheckIfPrime((digit - '0'))) primeAmount++; }
                                                if (((digit1 - digit6 < 0) && (job == (int)Job.Firefighter || job == (int)Job.Mechanic)) || ((job == (int)Job.Teacher || job == (int)Job.Cashier) && evenAmount <= 2)                               || (digit1 + digit2 + digit3 + digit4 + digit5 + digit6 >= 35 && (job == (int)Job.Baker || job == (int)Job.Policeperson))         || (evenAmount == 5))                                                                                                status = (int)MaritalStatus.Divorced;
                                                if (((digit2 * digit5 <= 20 && (job == (int)Job.Mechanic || job == (int)Job.Baker)))      || ((job == (int)Job.Policeperson || job == (int)Job.Cashier) && (CheckIfPrime(digit2) && CheckIfPrime(digit3))) || (digit4 % 3 == 0 && (job == (int)Job.Firefighter || job == (int)Job.Teacher))                                           || (evenAmount == 1))                                                                                                status = (status == -1) ? (int)MaritalStatus.Married : (int)MaritalStatus.Engaged;
                                                if ((evenAmount == 6 || evenAmount == 0)                                                 || (primeAmount == 0 || primeAmount == 6)                                                                  || (6 - evenAmount == primeAmount && (job == (int)Job.Mechanic || job == (int)Job.Firefighter || job == (int)Job.Cashier)) || (evenAmount == 6 - primeAmount && (job == (int)Job.Teacher || job == (int)Job.Policeperson || job == (int)Job.Baker)))   status = (status == -1) ? (int)MaritalStatus.Widowed : (int)MaritalStatus.Engaged;
                                                if (status == -1)                                                                                                                                                                                                                                                                                                                                                                                                                                          status = (int)MaritalStatus.Single;

                                                bool cond1 = false; bool cond2 = false; int statusAux = (status == (int)MaritalStatus.Engaged) ? 0 : status;
                                                foreach (char digit in label) { if (job == digit - '0') cond1 = true; if (statusAux == digit - '0') cond2 = true; }
                                                _class = (!cond1 && !cond2) ? (int)SocialClass.Lower : (cond1 ^ cond2) ? (int)SocialClass.Middle : (int)SocialClass.Upper;

                                                return new int[] { city, home, contract, job, status, _class }; }

    private bool CheckIfPrime(int num) { return (num == 2 || num == 3 || num == 5 || num == 7); }

    private IEnumerator CountDown() {   float timerDelta = _timer;
                                        while(timerDelta >= 0) {    yield return new WaitForSecondsRealtime(1f);
                                                                    timerDelta -= 1f;
                                                                    _timerBar.fillAmount = timerDelta / _timer; }
                                        _health--; GetComponent<AudioSource>().Stop(); GetComponent<AudioSource>().PlayOneShot(_incorrectAudio, 1.3f); NextCaller(); }

    private IEnumerator TransitionToNextCaller() {  _enableBoard = false; _roundsToWin--;
                                                    _currentCallers = new List<(GameObject, int[])> { _assignedPlucks[UnityEngine.Random.Range(0, _assignedPlucks.Count)], _assignedPlucks[UnityEngine.Random.Range(0, _assignedPlucks.Count)] };
                                                    while (_currentCallers[0] == _currentCallers[1]) _currentCallers[1] = _assignedPlucks[UnityEngine.Random.Range(0, _assignedPlucks.Count)];

                                                    yield return new WaitForSeconds(3);

                                                    _introAudioSource.PlayOneShot(_phoneRingingAudio, .15f);

                                                    while (_introAudioSource.isPlaying) yield return null;

                                                    GetComponent<AudioSource>().Play(); _introAudioSource.PlayOneShot(_personSpeakingAudio);
                                                    int[] callerId   = _currentCallers[0].labelId; _panelText.text = $"<color=green>City:</color> {(City)callerId[0]}\n<color=green>Home:</color>  {(Home)callerId[1]}\n<color=green>House Contract:</color> {(Contract)callerId[2]}\n<color=green>Job:</color>  {(Job)callerId[3]}\n<color=green>Marital Status:</color> {(MaritalStatus)callerId[4]}\n<color=green>Social Class:</color> {(SocialClass)callerId[5]}";
                                                    int[] receiverId = _currentCallers[1].labelId; _dialogueText.text = $"I'm looking for someone who lives in <color=#EDB974>{(City)receiverId[0]}</color>, in a <color=#EDB974>{(Contract)receiverId[2]}</color> <color=#EDB974>{(Home)receiverId[1]}</color> they are <color=#EDB974>{(MaritalStatus)receiverId[4]}</color>, <color=#EDB974>{(SocialClass)receiverId[5]}</color> class and work as a <color=#EDB974>{(Job)receiverId[3]}</color>.";
                                                    _enableBoard = true; _countDown = StartCoroutine(CountDown());} }
