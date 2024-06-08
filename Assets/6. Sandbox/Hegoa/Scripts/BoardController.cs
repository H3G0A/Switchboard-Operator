using System.Collections; using System.Collections.Generic; using UnityEngine; using TMPro;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<GameObject> _plucks = new(); 
    [SerializeField] List<GameObject> _hiddenConnectors = new();
    [SerializeField] List<string> _labelIds = new();
    List<(GameObject connector, GameObject pluck)> _activeConnectors = new();

    [SerializeField] GameObject _connectorPrefab; 
    [SerializeField] LineRenderer _lineRenderer;

    [SerializeField] int _health = 3;
    [SerializeField] List<GameObject> _currentCallers;
    [SerializeField] bool _enableBoard = false;

    enum City {Madrid, Lebrija, Seville, Cadiz, Toledo}
    enum Home {Duplex, Apartment, Chalet, Terraced}
    enum Contract {Rented, Property}
    enum Job {Mechanic, Professor, Firefighter, Baker, Police, Cashier}
    enum MaritalStatus {Divorced, Married, Widowed, Single}
    enum SocialClass {Low, Middle, High}
    
    private void Start()
    {
        SetLabels();
        NextCaller();
    }

    public void OnSwitchClick(GameObject button)
    {
        if (!_enableBoard) return;

        if (_activeConnectors.Count < 2)
        {
            _hiddenConnectors[0].transform.position = button.transform.position;
            _hiddenConnectors[0].SetActive(true);
            _activeConnectors.Add(new(_hiddenConnectors[0], button.transform.parent.gameObject));
            _hiddenConnectors.RemoveAt(0);
        }
        if (_activeConnectors.Count == 2)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, new(_activeConnectors[0].connector.transform.position.x, _activeConnectors[0].connector.transform.position.y, 0));
            _lineRenderer.SetPosition(1, new(_activeConnectors[1].connector.transform.position.x, _activeConnectors[1].connector.transform.position.y, 0));
        }
    }

    public void RemoveConnector(GameObject gameobject)
    {
        (GameObject connector, GameObject pluck) tuple = _activeConnectors.Find(x => x.connector == (_plucks[0]));
        if (tuple == (null, null)) { _activeConnectors.Remove(tuple); _hiddenConnectors.Add(gameobject); gameobject.SetActive(false); _lineRenderer.enabled = false; }
    }

    public void NextCaller()
    {
        _currentCallers = new List<GameObject> {_plucks[UnityEngine.Random.Range(0, _plucks.Count)], _plucks[UnityEngine.Random.Range(0, _plucks.Count)] };
        while (_currentCallers[0] == _currentCallers[1]) _currentCallers[1] = _plucks[UnityEngine.Random.Range(0, _plucks.Count)];
        _enableBoard = true;
    }

    public void ConfirmSelection()
    {
        if (_activeConnectors.Count < 2) return;
        if (!_currentCallers.Contains(_activeConnectors[0].pluck) || !_currentCallers.Contains(_activeConnectors[1].pluck)) _health--;
        
        RemoveConnector(_activeConnectors[0].connector); RemoveConnector(_activeConnectors[0].connector);
        NextCaller();
    }

    private void SetLabels()
    {
        foreach(GameObject pluck in _plucks) {
            string label; string labelId = CreateLabelId(label = Random.Range(0, 999999).ToString("D6"));
            while(_labelIds.Contains(labelId)) labelId = CreateLabelId(label = Random.Range(0, 999999).ToString("D6"));

            pluck.GetComponentInChildren<TextMeshProUGUI>().text = label; _labelIds.Add(labelId); }
    }

    private string CreateLabelId(string label)
    {
        int city = 0; int home = 0; int contract = 0; int job = 0; int status = -1; int _class = 0;


        int digits12 = int.Parse(label.Substring(0, 2));
        if      ((digits12 >= 00 && digits12 <= 08) || (digits12 >= 55 && digits12 <= 64) || digits12 == 93)                     city = (int)City.Madrid;
        else if ((digits12 >= 31 && digits12 <= 40) || (digits12 >= 71 && digits12 <= 74) || (digits12 >= 94 && digits12 <= 99)) city = (int)City.Lebrija;
        else if ((digits12 >= 16 && digits12 <= 22) || (digits12 >= 41 && digits12 <= 46) || (digits12 >= 82 && digits12 <= 88)) city = (int)City.Seville;
        else if ((digits12 >= 09 && digits12 <= 15) || (digits12 >= 65 && digits12 <= 70) || (digits12 >= 75 && digits12 <= 81)) city = (int)City.Cadiz;
        else if ((digits12 >= 23 && digits12 <= 30) || (digits12 >= 47 && digits12 <= 54) || (digits12 >= 89 && digits12 <= 92)) city = (int)City.Toledo;


        int digit3 = int.Parse(label.Substring(2, 1)); int digit4 = int.Parse(label.Substring(3, 1)); int digit3Parity = digit3 % 2; int digit4Parity = digit4 % 2;
        bool option1 = (city == (int)City.Seville || city == (int)City.Madrid); if (city == (int)City.Toledo)                 home = (int)Home.Apartment;
        else if ((option1 && digit3Parity == 0 && digit4Parity != 0) || (!option1 && digit3Parity == 0 && digit4Parity == 0)) home = (int)Home.Duplex;
        else if ((option1 && digit3Parity != 0 && digit4Parity == 0) || (!option1 && digit3Parity != 0 && digit4Parity != 0)) home = (int)Home.Apartment;
        else if ((option1 && digit3Parity == 0 && digit4Parity == 0) || (!option1 && digit3Parity != 0 && digit4Parity == 0)) home = (int)Home.Chalet;
        else if ((option1 && digit3Parity != 0 && digit4Parity != 0) || (!option1 && digit3Parity == 0 && digit4Parity != 0)) home = (int)Home.Terraced;


        int digit6 = int.Parse(label.Substring(5, 1));
        if (!CheckIfPrime(digit6)){ if      (city == (int)City.Lebrija || city == (int)City.Toledo)                                  contract = (int)Contract.Rented;
                                    else if (city == (int)City.Madrid && (home == (int)Home.Apartment || home == (int)Home.Terraced)) contract = (int)Contract.Property;
                                    else if (city == (int)City.Madrid && (home != (int)Home.Apartment && home != (int)Home.Terraced)) contract = (int)Contract.Rented;
                                    else if (city == (int)City.Seville && home != (int)Home.Chalet)                                  contract = (int)Contract.Property;
                                    else if (city == (int)City.Seville && home == (int)Home.Chalet)                                  contract = (int)Contract.Rented;
                                    else                                                                                           contract = (int)Contract.Property;}

        else                      { if      (home == (int)Home.Duplex)                                                              contract = (int)Contract.Property;
                                    else if (home != (int)Home.Terraced && (city == (int)City.Madrid || city == (int)City.Seville))   contract = (int)Contract.Rented;
                                    else if (city == (int)City.Lebrija || city == (int)City.Cadiz || city == (int)City.Toledo)        contract = (int)Contract.Property;
                                    else                                                                                           contract = (int)Contract.Rented;}


        int digit5 = int.Parse(label.Substring(4, 1)); 
        int sub = digit3 - digit5; int sum = digit4 + digit6;
        if      (sum == 0 || sum == 3 || sum == 6 || sum == 9 || sum == 12 || sum == 15 || sum == 18)  job = sub < 0 ? (int)Job.Mechanic    : (int)Job.Baker;
        else if (sum == 2 || sum == 5 || sum == 8 || sum == 11 || sum == 14 || sum == 17)              job = sub < 0 ? (int)Job.Professor   : (int)Job.Police;
        else if (sum == 1 || sum == 4 || sum == 7 || sum == 10 || sum == 13 || sum == 16)              job = sub < 0 ? (int)Job.Firefighter : (int)Job.Cashier;


        int digit1 = int.Parse(label.Substring(0, 1)); int digit2 = int.Parse(label.Substring(0, 1)); int evenAmount = 0; int primeAmount = 0;
        foreach (int digit in label.ToIntArray()) { if (digit % 2 == 0) evenAmount++; if (CheckIfPrime(digit)) primeAmount++; }

        if      ((digit1 - digit6 < 0) && (job == (int)Job.Firefighter || job == (int)Job.Mechanic) || ((job == (int)Job.Professor || job == (int)Job.Cashier) && evenAmount <= 2) || (digit1 + digit2 + digit3 + digit4 + digit5 + digit6 > 35 && (job == (int)Job.Baker || job == (int)Job.Police)) || (evenAmount == 5))                  status = (int)MaritalStatus.Divorced;
        if ((digit2 * digit5 < 20 && (job == (int)Job.Mechanic || job == (int)Job.Baker)) || ((job == (int)Job.Police || job == (int)Job.Cashier) && (CheckIfPrime(digit2) && CheckIfPrime(digit3))) || (digit4 % 3 == 0 && (job == (int)Job.Firefighter || job == (int)Job.Professor)) || (evenAmount == 1))                                status = status == -1 ? (int)MaritalStatus.Married : (int)MaritalStatus.Single;
        if ((evenAmount == 6 || evenAmount == 0) || (primeAmount == 0 || primeAmount == 6) || (6 - evenAmount == primeAmount && (job == (int)Job.Mechanic || job == (int)Job.Firefighter || job == (int)Job.Cashier)) || (evenAmount == 6 - primeAmount && (job == (int)Job.Professor || job == (int)Job.Police || job == (int)Job.Baker)))  status = status == -1 ? (int)MaritalStatus.Widowed : (int)MaritalStatus.Single;
        if (status == -1)                                                                                                                                                                                                                                                                                                                    status = (int)MaritalStatus.Single;


        if      (status == (int)MaritalStatus.Single)    _class = (job == (int)Job.Mechanic  || job == (int)Job.Professor || job == (int)Job.Firefighter)                      ? (int)SocialClass.Middle : (int)SocialClass.High;
        else if (status == (int)MaritalStatus.Divorced)  _class = (job == (int)Job.Mechanic  || job == (int)Job.Police    || job == (int)Job.Cashier || job == (int)Job.Baker) ? (int)SocialClass.Low    : (int)SocialClass.High;
        else if (status == (int)MaritalStatus.Married)   _class = (job == (int)Job.Professor || job == (int)Job.Firefighter)                                                   ? (int)SocialClass.Low    : (job == (int)Job.Police      || job == (int)Job.Cashier)                          ? (int)SocialClass.Middle : (int)SocialClass.High;
        else if (status == (int)MaritalStatus.Widowed)   _class = (job == (int)Job.Mechanic  || job == (int)Job.Police)                                                        ? (int)SocialClass.Low    : (job == (int)Job.Firefighter || job == (int)Job.Cashier || job == (int)Job.Baker) ? (int)SocialClass.Middle : (int)SocialClass.High;

        Debug.Log("Label: " + label);
        Debug.Log("Full info: " + (City)city + "; " + (Home)home + "; " + (Contract)contract + "; " + (Job)job + "; " + (MaritalStatus)status + "; " + (SocialClass)_class);
        return ("" + city + home + contract + job + status + _class);
    }

    private bool CheckIfPrime(int num) { return (num == 2 || num == 3 || num == 5 || num == 7); }
}
