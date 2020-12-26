using Jypeli;
using System.Collections.Generic;
using System.Linq;
using System;

/// @author Jenni Arovaara
/// @version 4.4.2019
/// <summary>
/// Putoavien roskien lista ja toiminnot.
/// </summary>
class Roskalista
{
    /// <summary>
    /// Lista putoavista roskista.
    /// </summary>
    private List<PhysicsObject> Lista = new List<PhysicsObject>();

    private PhysicsObject _valittu;

    /// <summary>
    /// Valittu roska eli alin roska. 
    /// </summary>
    public PhysicsObject Valittu
    {
        get => _valittu;
        private set {
            _valittu = value;
            ValittuMuuttui?.Invoke();
        }
    }

    /// <summary>
    /// Toiminnot, jotka ajetaan, kun valittu roska muuttuu.
    /// </summary>
    public event Action ValittuMuuttui;

    /// <summary>
    /// Tyhjennä roskalista.
    /// </summary>
    public void Tyhjenna()
    {
        Valittu = null;
        Lista.ForEach(roska => roska.Destroy());
        Lista.Clear();
    }

    /// <summary>
    /// Poista roska listasta.
    /// </summary>
    /// <param name="roska">Roska</param>
    public void Poista(PhysicsObject roska)
    {
        Lista.Remove(roska);
        if (roska == Valittu)
        {
            Valittu = Lista.FirstOrDefault();
        }
    }

    /// <summary>
    /// Lisää roska listalle.
    /// </summary>
    /// <param name="roska">Roska</param>
    public void Lisaa(PhysicsObject roska)
    {
        Lista.Add(roska);
        if (Valittu == null)
        {
            Valittu = roska;
        }
    }
}
