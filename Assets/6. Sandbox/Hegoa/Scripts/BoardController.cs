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
    [SerializeField] GameObject _currentCaller; 
    [SerializeField] GameObject _currentReceiver; 
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
        CreateLabel();
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
        foreach ((GameObject connector, GameObject pluck) tuple in _activeConnectors)
        {
            if (tuple.connector == gameobject) 
            { 
                _activeConnectors.Remove(tuple);
                break;
            }
        }

        _hiddenConnectors.Add(gameobject);
        gameobject.SetActive(false);
        _lineRenderer.enabled = false;
    }

    public void NextCaller()
    {
        _currentCaller = _plucks[UnityEngine.Random.Range(0, _plucks.Count)];

        do {
            _currentReceiver = _plucks[UnityEngine.Random.Range(0, _plucks.Count)];
        } while (_currentReceiver == _currentCaller);

        _enableBoard = true;
    }

    public void ConfirmSelection()
    {
        if (_activeConnectors.Count < 2) return;

        RemoveConnector(_activeConnectors[0].connector);
        RemoveConnector(_activeConnectors[0].connector);

        int matches = 0;
        foreach ((GameObject connector, GameObject pluck) in _activeConnectors) if (pluck == _currentCaller || pluck == _currentReceiver) matches++;

        if (matches < 2) SetHealth(_health - 1);
        NextCaller();
    }

    private void SetHealth(int health)
    {
        this._health = health;
    }

    private void SetLabels()
    {
        foreach(GameObject pluck in _plucks)
        {
            string label;
            bool isValid;
            do
            {
                isValid = true;
                label = Random.Range(0, 999999).ToString("D6");
                foreach (GameObject p in _plucks)
                {
                    if (label == p.GetComponentInChildren<TextMeshProUGUI>().text) isValid = false;

                }
            } while (!isValid);

            pluck.GetComponentInChildren<TextMeshProUGUI>().text = label;
        }
    }

    private void CreateLabel1()
    {
        string label = "";
        int id1 = Random.Range(0, 5); int id2 = Random.Range(0, 4); int id3 = Random.Range(0, 2); 
        int id4 = Random.Range(0, 6); int id5 = Random.Range(0, 4); int id6 = Random.Range(0, 3);
        string labelId = "" + id1 + id2 + id3 + id4 + id5 + id6;

        
    }

    private void CreateLabel()
    {
        string label = Random.Range(0, 999999).ToString("D6");
        int id1 = 0; int id2 = 0; int id3 = 0; int id4 = 0; int id5 = 0; int id6 = 0;


        int digits12 = int.Parse(label.Substring(0, 2));
        if      ((digits12 >= 00 && digits12 <= 08) || (digits12 >= 55 && digits12 <= 64) || digits12 == 93)                     id1 = (int)City.Madrid;
        else if ((digits12 >= 31 && digits12 <= 40) || (digits12 >= 71 && digits12 <= 74) || (digits12 >= 94 && digits12 <= 99)) id1 = (int)City.Lebrija;
        else if ((digits12 >= 16 && digits12 <= 22) || (digits12 >= 41 && digits12 <= 46) || (digits12 >= 82 && digits12 <= 88)) id1 = (int)City.Seville;
        else if ((digits12 >= 09 && digits12 <= 15) || (digits12 >= 65 && digits12 <= 70) || (digits12 >= 75 && digits12 <= 81)) id1 = (int)City.Cadiz;
        else if ((digits12 >= 23 && digits12 <= 30) || (digits12 >= 47 && digits12 <= 54) || (digits12 >= 89 && digits12 <= 92)) id1 = (int)City.Toledo;


        int digit3 = int.Parse(label.Substring(2, 1)); int digit3Parity = digit3 % 2;
        int digit4 = int.Parse(label.Substring(3, 1)); int digit4Parity = digit4 % 2;
        if      (digit3Parity == 0 && digit4Parity != 0) id2 = 0; //Madrid-Sevilla  // Duplex         //Cadiz-Lebrija   //Adosado
        else if (digit3Parity != 0 && digit4Parity == 0) id2 = 1;                   // Apartamento                      // Chalet
        else if (digit3Parity == 0 && digit4Parity == 0) id2 = 2;                   // Chalet                           // Duplex
        else if (digit3Parity != 0 && digit4Parity != 0) id2 = 3;                   // Adosado                          //Apartamento
        if (id1 == (int)City.Toledo || id1 == (int)City.Seville) id2 += 4;


        int digit6 = int.Parse(label.Substring(5, 1));
        if (!CheckIfPrime(digit6)){ if      (id1 == (int)City.Lebrija || id1 == (int)City.Toledo)                           id3 = (int)Contract.Rented;
                                    else if (id2 == 1 || id2 == 3)                                                          id3 = (int)Contract.Property;
                                    else if (id2 != 1 && id2 != 3)                                                          id3 = (int)Contract.Rented;
                                    else if (id1 == (int)City.Seville && id2 != 2)                                          id3 = (int)Contract.Property;
                                    else if (id2 == 2)                                                                      id3 = (int)Contract.Rented;
                                    else                                                                                    id3 = (int)Contract.Property;}

        else                      { if      (id2 == 0 || id2 == 6)                                                          id3 = (int)Contract.Property;
                                    else if (id2 != 3 && (id1 == (int)City.Madrid || id1 == (int)City.Seville))             id3 = (int)Contract.Rented;
                                    else if (id1 == (int)City.Lebrija || id1 == (int)City.Cadiz || id1 == (int)City.Toledo) id3 = (int)Contract.Property;
                                    else                                                                                    id3 = (int)Contract.Rented;}


        int digit5 = int.Parse(label.Substring(4, 1)); 
        int sub = digit3 - digit5; int sum = digit4 - digit6;
        if(sub < 0) { if      (sum <= 6)  id4 = (int)Job.Mechanic;
                      else if (sum <= 11) id4 = (int)Job.Professor;
                      else if (sum <= 18) id4 = (int)Job.Firefighter;}

        else        { if      (sum <= 4)  id4 = (int)Job.Baker;  
                      else if (sum <= 9)  id4 = (int)Job.Police;  
                      else if (sum <= 18) id4 = (int)Job.Cashier;}


        int digit1 = int.Parse(label.Substring(0, 1)); int digit2 = int.Parse(label.Substring(0, 1)); int evenAmount = 0; int primeAmount = 0;
        foreach (int digit in label.ToIntArray()) { if (digit % 2 == 0) evenAmount++; if (CheckIfPrime(digit)) primeAmount++; }

        if      ((digit1 - digit6 < 0) && (id4 == (int)Job.Firefighter || id4 == (int)Job.Mechanic) || ((id4 == (int)Job.Professor || id4 == (int)Job.Cashier) && evenAmount >= 2) || (digit1 + digit2 + digit3 + digit4 + digit5 + digit6 >= 25 && (id4 == (int)Job.Baker || id4 == (int)Job.Police)) || (evenAmount >= 5)) id5 = (int)MaritalStatus.Divorced;
        else if ((digit2 * digit5 < 40 && (id4 == (int)Job.Mechanic || id4 == (int)Job.Baker)) || ((id4 == (int)Job.Police || id4 == (int)Job.Cashier) && (CheckIfPrime(digit2))) || (digit4%3 == 0 && (id4 == (int)Job.Firefighter || id4 == (int)Job.Professor)) || (evenAmount <= 1))                                     id5 = (int)MaritalStatus.Married;
        else if ((evenAmount == 6 || evenAmount == 0) || (primeAmount == 0 || primeAmount == 6) || (evenAmount == 3) || (primeAmount == 3))                                                                                                                                                                                  id5 = (int)MaritalStatus.Widowed;
        else id5 = (int)MaritalStatus.Single;


        //if () id6 = (int)SocialClass.Low;
        //else if (id4 == (int)Job.Mechanic && id4 == (int)MaritalStatus.Single) id6 = (int)SocialClass.Middle;
        //else id6 = (int)SocialClass.High;


        _labelIds.Add("" + id1 + id2 + id3 + id4 + id5 + id6);
    }

    private bool CheckIfPrime(int num)
    {
        if (num == 2 || num == 3 || num == 5 || num == 7) return true; else return false;
    }
}
