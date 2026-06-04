using System;
using MMI_SP.Agency.Office.Ambient;
using GTA.Chrono;
using MMI_SP.Helpers;

namespace MMI_SP.Agency.Office
{
    internal class Manager
    {
        // ==========================================
        // BLOQUE 1: Variables de la clase y creación del objeto
        // ==========================================
        private Handler _office;
        private TimeSpan _officeLastCreation = new TimeSpan(0);
        private ItemsManager.OfficeItemsCollection _officeLastCollection = new ItemsManager.OfficeItemsCollection();

        internal string CurrentCollectionType => _office?.ItemsCollection.Type.ToString() ?? "None";
        internal int CurrentCollectionCount => _office?.ItemsCollection.Count ?? 0;

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal void CreateOffice()
        {
            int currentHour = GameClock.Hour;

            if (_officeLastCreation.Hours == currentHour && _officeLastCollection.Count > 0)
            {
                _office = new Handler(_officeLastCollection);
            }
            else
            {
                _office = new Handler();
                _officeLastCreation = new TimeSpan(currentHour, 0, 0);
                _officeLastCollection?.DeleteItems();
                _officeLastCollection = new ItemsManager.OfficeItemsCollection(_office.ItemsCollection);
            }
        }

        internal void UpdateSpeechTimer()
        {
            if (_office == null)
            {
                Timers.Reset("RandomSpeech");
                return;
            }

            Timers.CheckAndExecuteRandom("RandomSpeech", () => { /* Aquí iría la acción del discurso, si la hubiera */ });
        }

        internal void DestroyOffice()
        {
            _office?.CleanOffice();
            _office = null;
            Timers.Reset("RandomSpeech");
        }
    }
}