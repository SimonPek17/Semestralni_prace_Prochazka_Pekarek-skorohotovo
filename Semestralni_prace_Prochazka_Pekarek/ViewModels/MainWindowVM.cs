using DataEntity;
using DataEntity.Data;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using Semestralni_prace_Prochazka_Pekarek.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Semestralni_prace_Prochazka_Pekarek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowVM
    {
        // DbContext se vytváří až v Initialize / EnsureDb (kvůli XAML designeru)
        private SkladContext? _db;

        // Indikátory stavu DB
        public bool DbAvailable { get; private set; } = false;
        public string? DbError { get; private set; }

        // propojení horních tabů s obsahem
        public int SelectedTabIndex { get; set; } = 0;

        // ====== ZAKAZNICI ======
        public ObservableCollection<Zakaznici> Zakaznici { get; set; } = new();
        public Zakaznici? SelectedZakaznik { get; set; }
        public Zakaznici EditZakaznik { get; set; } = new();

        public RelayCommand AddZakaznikCommand { get; }
        public RelayCommand UpdateZakaznikCommand { get; }
        public RelayCommand DeleteZakaznikCommand { get; }

        // ====== VOZIDLA ======
        public ObservableCollection<Vozidla> Vozidla { get; set; } = new();
        public Vozidla? SelectedVozidlo { get; set; }
        public Vozidla EditVozidlo { get; set; } = new();

        public RelayCommand AddVozidloCommand { get; }
        public RelayCommand UpdateVozidloCommand { get; }
        public RelayCommand DeleteVozidloCommand { get; }

        // ====== PRONAJMY ======
        public ObservableCollection<Pronajmy> Pronajmy { get; set; } = new();
        public Pronajmy? SelectedPronajem { get; set; }
        public Pronajmy EditPronajem { get; set; } = new();

        // ComboBox pick listy
        public ObservableCollection<Zakaznici> ZakazniciPick { get; set; } = new();
        public ObservableCollection<Vozidla> VozidlaPick { get; set; } = new();
        public ObservableCollection<Pronajmy> PronajmyPick { get; set; } = new();

        public Zakaznici? PickZakaznik { get; set; }
        public Vozidla? PickVozidlo { get; set; }
        public Pronajmy? PickPronajem { get; set; }

        public RelayCommand AddPronajemCommand { get; }
        public RelayCommand UpdatePronajemCommand { get; }
        public RelayCommand DeletePronajemCommand { get; }

        // ====== POKUTY ======
        public ObservableCollection<Pokuty> Pokuty { get; set; } = new();
        public Pokuty? SelectedPokuta { get; set; }
        public Pokuty EditPokuta { get; set; } = new();

        public RelayCommand AddPokutaCommand { get; }
        public RelayCommand UpdatePokutaCommand { get; }
        public RelayCommand DeletePokutaCommand { get; }

        public MainWindowVM()
        {
            // ZAKAZNICI
            AddZakaznikCommand = new RelayCommand(AddZakaznik, () => DbAvailable);
            UpdateZakaznikCommand = new RelayCommand(UpdateZakaznik, () => DbAvailable && SelectedZakaznik != null);
            DeleteZakaznikCommand = new RelayCommand(DeleteZakaznik, () => DbAvailable && SelectedZakaznik != null);

            // VOZIDLA
            AddVozidloCommand = new RelayCommand(AddVozidlo, () => DbAvailable);
            UpdateVozidloCommand = new RelayCommand(UpdateVozidlo, () => DbAvailable && SelectedVozidlo != null);
            DeleteVozidloCommand = new RelayCommand(DeleteVozidlo, () => DbAvailable && SelectedVozidlo != null);

            // PRONAJMY
            AddPronajemCommand = new RelayCommand(AddPronajem, () => DbAvailable && PickZakaznik != null && PickVozidlo != null);
            UpdatePronajemCommand = new RelayCommand(UpdatePronajem, () => DbAvailable && SelectedPronajem != null && PickZakaznik != null && PickVozidlo != null);
            DeletePronajemCommand = new RelayCommand(DeletePronajem, () => DbAvailable && SelectedPronajem != null);

            // POKUTY
            AddPokutaCommand = new RelayCommand(AddPokuta, () => DbAvailable && PickPronajem != null);
            UpdatePokutaCommand = new RelayCommand(UpdatePokuta, () => DbAvailable && SelectedPokuta != null && PickPronajem != null);
            DeletePokutaCommand = new RelayCommand(DeletePokuta, () => DbAvailable && SelectedPokuta != null);
        }

        /// <summary>
        /// Zavolej z MainWindow.Loaded (nebo hned po vytvoření VM).
        /// </summary>
        public void Initialize()
        {
            EnsureDb(forceReload: true);
            RaiseAllCanExecuteChanged();
        }

        // =========================
        // DB SAFE INIT
        // =========================
        private bool EnsureDb(bool forceReload = false)
        {
            try
            {
                if (_db != null && !forceReload && DbAvailable)
                    return true;

                _db = new SkladContext();

                // pokud nejde connect, zkus vytvořit
                try
                {
                    if (!_db.Database.CanConnect())
                        _db.Database.EnsureCreated();
                }
                catch
                {
                    // necháme zpracovat níže
                }

                if (!_db.Database.CanConnect())
                {
                    DbAvailable = false;
                    DbError = "Nelze se připojit k databázi. Zkontrolujte connection string a oprávnění.";
                    RaiseAllCanExecuteChanged();
                    return false;
                }

                DbAvailable = true;
                DbError = null;

                LoadAll();
                RaiseAllCanExecuteChanged();
                return true;
            }
            catch (Exception ex)
            {
                DbAvailable = false;
                DbError = ex.Message;
                RaiseAllCanExecuteChanged();
                return false;
            }
        }

        private void RaiseAllCanExecuteChanged()
        {
            AddZakaznikCommand.RaiseCanExecuteChanged();
            UpdateZakaznikCommand.RaiseCanExecuteChanged();
            DeleteZakaznikCommand.RaiseCanExecuteChanged();

            AddVozidloCommand.RaiseCanExecuteChanged();
            UpdateVozidloCommand.RaiseCanExecuteChanged();
            DeleteVozidloCommand.RaiseCanExecuteChanged();

            AddPronajemCommand.RaiseCanExecuteChanged();
            UpdatePronajemCommand.RaiseCanExecuteChanged();
            DeletePronajemCommand.RaiseCanExecuteChanged();

            AddPokutaCommand.RaiseCanExecuteChanged();
            UpdatePokutaCommand.RaiseCanExecuteChanged();
            DeletePokutaCommand.RaiseCanExecuteChanged();
        }

        // =========================
        // LOADERS
        // =========================
        private void LoadAll()
        {
            if (_db == null) return;

            LoadZakaznici();
            LoadVozidla();
            LoadPronajmy();
            LoadPokuty();
            RefreshPickLists();
        }

        private void LoadZakaznici()
        {
            if (_db == null) return;
            Zakaznici = new ObservableCollection<Zakaznici>(_db.Zakaznici.OrderBy(z => z.ID_Zakaznik).ToList());
        }

        private void LoadVozidla()
        {
            if (_db == null) return;
            Vozidla = new ObservableCollection<Vozidla>(_db.Vozidla.OrderBy(v => v.ID_Auto).ToList());
        }

        private void LoadPronajmy()
        {
            if (_db == null) return;
            Pronajmy = new ObservableCollection<Pronajmy>(_db.Pronajmy
                .Include(p => p.Zakaznik)
                .Include(p => p.Vozidlo)
                .OrderBy(p => p.ID_Pronajem)
                .ToList());
        }

        private void LoadPokuty()
        {
            if (_db == null) return;
            Pokuty = new ObservableCollection<Pokuty>(_db.Pokuty
                .Include(p => p.Pronajem).ThenInclude(pr => pr.Zakaznik)
                .Include(p => p.Pronajem).ThenInclude(pr => pr.Vozidlo)
                .OrderBy(p => p.ID_Pokuta)
                .ToList());
        }

        private void RefreshPickLists()
        {
            if (_db == null) return;

            ZakazniciPick = new ObservableCollection<Zakaznici>(_db.Zakaznici
                .OrderBy(z => z.Prijmeni).ThenBy(z => z.Jmeno)
                .ToList());

            VozidlaPick = new ObservableCollection<Vozidla>(_db.Vozidla
                .OrderBy(v => v.SPZ)
                .ToList());

            PronajmyPick = new ObservableCollection<Pronajmy>(_db.Pronajmy
                .Include(p => p.Zakaznik)
                .Include(p => p.Vozidlo)
                .OrderByDescending(p => p.ID_Pronajem)
                .ToList());
        }

        // =========================
        // ZAKAZNICI
        // =========================
        public void OnSelectedZakaznikChanged()
        {
            UpdateZakaznikCommand.RaiseCanExecuteChanged();
            DeleteZakaznikCommand.RaiseCanExecuteChanged();

            if (SelectedZakaznik == null) return;

            EditZakaznik = new Zakaznici
            {
                ID_Zakaznik = SelectedZakaznik.ID_Zakaznik,
                Jmeno = SelectedZakaznik.Jmeno,
                Prijmeni = SelectedZakaznik.Prijmeni,
                DatumNarozeni = SelectedZakaznik.DatumNarozeni,
                Adresa = SelectedZakaznik.Adresa,
                Telefon = SelectedZakaznik.Telefon,
                Email = SelectedZakaznik.Email,
                PocetPronajmu = SelectedZakaznik.PocetPronajmu
            };
        }

        private void AddZakaznik()
        {
            if (!EnsureDb()) return;

            var entity = new Zakaznici
            {
                Jmeno = EditZakaznik.Jmeno,
                Prijmeni = EditZakaznik.Prijmeni,
                DatumNarozeni = EditZakaznik.DatumNarozeni == default ? DateTime.Now : EditZakaznik.DatumNarozeni,
                Adresa = EditZakaznik.Adresa,
                Telefon = EditZakaznik.Telefon,
                Email = EditZakaznik.Email,
                PocetPronajmu = EditZakaznik.PocetPronajmu
            };

            _db!.Zakaznici.Add(entity);
            _db.SaveChanges();

            Zakaznici.Add(entity);
            RefreshPickLists();
            EditZakaznik = new Zakaznici();
        }

        private void UpdateZakaznik()
        {
            if (!EnsureDb()) return;
            if (SelectedZakaznik == null) return;

            var entity = _db!.Zakaznici.First(z => z.ID_Zakaznik == SelectedZakaznik.ID_Zakaznik);

            entity.Jmeno = EditZakaznik.Jmeno;
            entity.Prijmeni = EditZakaznik.Prijmeni;
            entity.DatumNarozeni = EditZakaznik.DatumNarozeni;
            entity.Adresa = EditZakaznik.Adresa;
            entity.Telefon = EditZakaznik.Telefon;
            entity.Email = EditZakaznik.Email;
            entity.PocetPronajmu = EditZakaznik.PocetPronajmu;

            _db.SaveChanges();

            SelectedZakaznik.Jmeno = entity.Jmeno;
            SelectedZakaznik.Prijmeni = entity.Prijmeni;
            SelectedZakaznik.DatumNarozeni = entity.DatumNarozeni;
            SelectedZakaznik.Adresa = entity.Adresa;
            SelectedZakaznik.Telefon = entity.Telefon;
            SelectedZakaznik.Email = entity.Email;
            SelectedZakaznik.PocetPronajmu = entity.PocetPronajmu;

            RefreshPickLists();
        }

        private void DeleteZakaznik()
        {
            if (!EnsureDb()) return;
            if (SelectedZakaznik == null) return;

            var entity = _db!.Zakaznici.First(z => z.ID_Zakaznik == SelectedZakaznik.ID_Zakaznik);

            _db.Zakaznici.Remove(entity);
            _db.SaveChanges();

            Zakaznici.Remove(SelectedZakaznik);
            SelectedZakaznik = null;
            EditZakaznik = new Zakaznici();

            RefreshPickLists();
            UpdateZakaznikCommand.RaiseCanExecuteChanged();
            DeleteZakaznikCommand.RaiseCanExecuteChanged();
        }

        // =========================
        // VOZIDLA
        // =========================
        public void OnSelectedVozidloChanged()
        {
            UpdateVozidloCommand.RaiseCanExecuteChanged();
            DeleteVozidloCommand.RaiseCanExecuteChanged();

            if (SelectedVozidlo == null) return;

            EditVozidlo = new Vozidla
            {
                ID_Auto = SelectedVozidlo.ID_Auto,
                SPZ = SelectedVozidlo.SPZ,
                Znacka = SelectedVozidlo.Znacka,
                Model = SelectedVozidlo.Model,
                RokVyroby = SelectedVozidlo.RokVyroby,
                StavTachometru = SelectedVozidlo.StavTachometru,
                DenniSazba = SelectedVozidlo.DenniSazba,
                Stav = SelectedVozidlo.Stav
            };
        }

        private void AddVozidlo()
        {
            if (!EnsureDb()) return;

            var entity = new Vozidla
            {
                SPZ = EditVozidlo.SPZ,
                Znacka = EditVozidlo.Znacka,
                Model = EditVozidlo.Model,
                RokVyroby = EditVozidlo.RokVyroby,
                StavTachometru = EditVozidlo.StavTachometru,
                DenniSazba = EditVozidlo.DenniSazba,
                Stav = EditVozidlo.Stav
            };

            _db!.Vozidla.Add(entity);
            _db.SaveChanges();

            Vozidla.Add(entity);
            RefreshPickLists();
            EditVozidlo = new Vozidla();
        }

        private void UpdateVozidlo()
        {
            if (!EnsureDb()) return;
            if (SelectedVozidlo == null) return;

            var entity = _db!.Vozidla.First(v => v.ID_Auto == SelectedVozidlo.ID_Auto);

            entity.SPZ = EditVozidlo.SPZ;
            entity.Znacka = EditVozidlo.Znacka;
            entity.Model = EditVozidlo.Model;
            entity.RokVyroby = EditVozidlo.RokVyroby;
            entity.StavTachometru = EditVozidlo.StavTachometru;
            entity.DenniSazba = EditVozidlo.DenniSazba;
            entity.Stav = EditVozidlo.Stav;

            _db.SaveChanges();

            SelectedVozidlo.SPZ = entity.SPZ;
            SelectedVozidlo.Znacka = entity.Znacka;
            SelectedVozidlo.Model = entity.Model;
            SelectedVozidlo.RokVyroby = entity.RokVyroby;
            SelectedVozidlo.StavTachometru = entity.StavTachometru;
            SelectedVozidlo.DenniSazba = entity.DenniSazba;
            SelectedVozidlo.Stav = entity.Stav;

            RefreshPickLists();
        }

        private void DeleteVozidlo()
        {
            if (!EnsureDb()) return;
            if (SelectedVozidlo == null) return;

            var entity = _db!.Vozidla.First(v => v.ID_Auto == SelectedVozidlo.ID_Auto);

            _db.Vozidla.Remove(entity);
            _db.SaveChanges();

            Vozidla.Remove(SelectedVozidlo);
            SelectedVozidlo = null;
            EditVozidlo = new Vozidla();

            RefreshPickLists();
            UpdateVozidloCommand.RaiseCanExecuteChanged();
            DeleteVozidloCommand.RaiseCanExecuteChanged();
        }

        // =========================
        // PRONAJMY
        // =========================
        public void OnSelectedPronajemChanged()
        {
            UpdatePronajemCommand.RaiseCanExecuteChanged();
            DeletePronajemCommand.RaiseCanExecuteChanged();

            if (SelectedPronajem == null) return;

            PickZakaznik = ZakazniciPick.FirstOrDefault(z => z.ID_Zakaznik == SelectedPronajem.ID_Zakaznik);
            PickVozidlo = VozidlaPick.FirstOrDefault(v => v.ID_Auto == SelectedPronajem.ID_Auto);

            EditPronajem = new Pronajmy
            {
                ID_Pronajem = SelectedPronajem.ID_Pronajem,
                ID_Zakaznik = SelectedPronajem.ID_Zakaznik,
                ID_Auto = SelectedPronajem.ID_Auto,
                DatumOd = SelectedPronajem.DatumOd,
                DatumDo = SelectedPronajem.DatumDo,
                DelkaDni = SelectedPronajem.DelkaDni,
                CenaCelkem = SelectedPronajem.CenaCelkem
            };

            AddPronajemCommand.RaiseCanExecuteChanged();
            UpdatePronajemCommand.RaiseCanExecuteChanged();
        }

        public void OnPickZakaznikChanged()
        {
            AddPronajemCommand.RaiseCanExecuteChanged();
            UpdatePronajemCommand.RaiseCanExecuteChanged();
        }

        public void OnPickVozidloChanged()
        {
            AddPronajemCommand.RaiseCanExecuteChanged();
            UpdatePronajemCommand.RaiseCanExecuteChanged();
            RecalculatePronajem();
        }

        public void OnEditPronajemChanged()
        {
            RecalculatePronajem();
        }

        public void OnEditPronajemDatumOdChanged() => RecalculatePronajem();
        public void OnEditPronajemDatumDoChanged() => RecalculatePronajem();

        private void RecalculatePronajem()
        {
            if (EditPronajem == null) return;

            var from = EditPronajem.DatumOd.Date;
            var to = EditPronajem.DatumDo.Date;

            if (to < from) to = from;

            var days = (to - from).Days;
            if (days < 1) days = 1;

            EditPronajem.DelkaDni = days;

            if (PickVozidlo != null)
                EditPronajem.CenaCelkem = PickVozidlo.DenniSazba * days;
        }

        private void AddPronajem()
        {
            if (!EnsureDb()) return;
            if (PickZakaznik == null || PickVozidlo == null) return;

            RecalculatePronajem();

            var entity = new Pronajmy
            {
                ID_Zakaznik = PickZakaznik.ID_Zakaznik,
                ID_Auto = PickVozidlo.ID_Auto,
                DatumOd = EditPronajem.DatumOd == default ? DateTime.Now : EditPronajem.DatumOd,
                DatumDo = EditPronajem.DatumDo == default ? DateTime.Now.AddDays(1) : EditPronajem.DatumDo,
                DelkaDni = EditPronajem.DelkaDni,
                CenaCelkem = EditPronajem.CenaCelkem
            };

            _db!.Pronajmy.Add(entity);
            _db.SaveChanges();

            // reload s navigacemi
            var loaded = _db.Pronajmy
                .Include(p => p.Zakaznik)
                .Include(p => p.Vozidlo)
                .First(p => p.ID_Pronajem == entity.ID_Pronajem);

            Pronajmy.Add(loaded);

            RefreshPickLists();
            EditPronajem = new Pronajmy { DatumOd = DateTime.Now, DatumDo = DateTime.Now.AddDays(1) };
            PickZakaznik = null;
            PickVozidlo = null;

            AddPronajemCommand.RaiseCanExecuteChanged();
            UpdatePronajemCommand.RaiseCanExecuteChanged();
        }

        private void UpdatePronajem()
        {
            if (!EnsureDb()) return;
            if (SelectedPronajem == null || PickZakaznik == null || PickVozidlo == null) return;

            RecalculatePronajem();

            var entity = _db!.Pronajmy.First(p => p.ID_Pronajem == SelectedPronajem.ID_Pronajem);

            entity.ID_Zakaznik = PickZakaznik.ID_Zakaznik;
            entity.ID_Auto = PickVozidlo.ID_Auto;
            entity.DatumOd = EditPronajem.DatumOd;
            entity.DatumDo = EditPronajem.DatumDo;
            entity.DelkaDni = EditPronajem.DelkaDni;
            entity.CenaCelkem = EditPronajem.CenaCelkem;

            _db.SaveChanges();

            var loaded = _db.Pronajmy
                .Include(p => p.Zakaznik)
                .Include(p => p.Vozidlo)
                .First(p => p.ID_Pronajem == entity.ID_Pronajem);

            SelectedPronajem.ID_Zakaznik = loaded.ID_Zakaznik;
            SelectedPronajem.ID_Auto = loaded.ID_Auto;
            SelectedPronajem.DatumOd = loaded.DatumOd;
            SelectedPronajem.DatumDo = loaded.DatumDo;
            SelectedPronajem.DelkaDni = loaded.DelkaDni;
            SelectedPronajem.CenaCelkem = loaded.CenaCelkem;
            SelectedPronajem.Zakaznik = loaded.Zakaznik;
            SelectedPronajem.Vozidlo = loaded.Vozidlo;

            RefreshPickLists();
        }

        private void DeletePronajem()
        {
            if (!EnsureDb()) return;
            if (SelectedPronajem == null) return;

            var entity = _db!.Pronajmy.First(p => p.ID_Pronajem == SelectedPronajem.ID_Pronajem);

            _db.Pronajmy.Remove(entity);
            _db.SaveChanges();

            Pronajmy.Remove(SelectedPronajem);
            SelectedPronajem = null;
            EditPronajem = new Pronajmy { DatumOd = DateTime.Now, DatumDo = DateTime.Now.AddDays(1) };
            PickZakaznik = null;
            PickVozidlo = null;

            RefreshPickLists();
            UpdatePronajemCommand.RaiseCanExecuteChanged();
            DeletePronajemCommand.RaiseCanExecuteChanged();
        }

        // =========================
        // POKUTY
        // =========================
        public void OnSelectedPokutaChanged()
        {
            UpdatePokutaCommand.RaiseCanExecuteChanged();
            DeletePokutaCommand.RaiseCanExecuteChanged();

            if (SelectedPokuta == null) return;

            PickPronajem = PronajmyPick.FirstOrDefault(p => p.ID_Pronajem == SelectedPokuta.ID_Pronajem);

            EditPokuta = new Pokuty
            {
                ID_Pokuta = SelectedPokuta.ID_Pokuta,
                ID_Pronajem = SelectedPokuta.ID_Pronajem,
                DatumPokuty = SelectedPokuta.DatumPokuty,
                Castka = SelectedPokuta.Castka,
                Duvod = SelectedPokuta.Duvod
            };

            AddPokutaCommand.RaiseCanExecuteChanged();
            UpdatePokutaCommand.RaiseCanExecuteChanged();
        }

        public void OnPickPronajemChanged()
        {
            AddPokutaCommand.RaiseCanExecuteChanged();
            UpdatePokutaCommand.RaiseCanExecuteChanged();
        }

        private void AddPokuta()
        {
            if (!EnsureDb()) return;
            if (PickPronajem == null) return;

            var entity = new Pokuty
            {
                ID_Pronajem = PickPronajem.ID_Pronajem,
                DatumPokuty = EditPokuta.DatumPokuty == default ? DateTime.Now : EditPokuta.DatumPokuty,
                Castka = EditPokuta.Castka,
                Duvod = EditPokuta.Duvod
            };

            _db!.Pokuty.Add(entity);
            _db.SaveChanges();

            var loaded = _db.Pokuty
                .Include(p => p.Pronajem).ThenInclude(pr => pr.Zakaznik)
                .Include(p => p.Pronajem).ThenInclude(pr => pr.Vozidlo)
                .First(p => p.ID_Pokuta == entity.ID_Pokuta);

            Pokuty.Add(loaded);

            EditPokuta = new Pokuty { DatumPokuty = DateTime.Now };
            PickPronajem = null;

            RefreshPickLists();
            AddPokutaCommand.RaiseCanExecuteChanged();
            UpdatePokutaCommand.RaiseCanExecuteChanged();
        }

        private void UpdatePokuta()
        {
            if (!EnsureDb()) return;
            if (SelectedPokuta == null || PickPronajem == null) return;

            var entity = _db!.Pokuty.First(p => p.ID_Pokuta == SelectedPokuta.ID_Pokuta);

            entity.ID_Pronajem = PickPronajem.ID_Pronajem;
            entity.DatumPokuty = EditPokuta.DatumPokuty;
            entity.Castka = EditPokuta.Castka;
            entity.Duvod = EditPokuta.Duvod;

            _db.SaveChanges();

            var loaded = _db.Pokuty
                .Include(p => p.Pronajem).ThenInclude(pr => pr.Zakaznik)
                .Include(p => p.Pronajem).ThenInclude(pr => pr.Vozidlo)
                .First(p => p.ID_Pokuta == entity.ID_Pokuta);

            SelectedPokuta.ID_Pronajem = loaded.ID_Pronajem;
            SelectedPokuta.DatumPokuty = loaded.DatumPokuty;
            SelectedPokuta.Castka = loaded.Castka;
            SelectedPokuta.Duvod = loaded.Duvod;
            SelectedPokuta.Pronajem = loaded.Pronajem;

            RefreshPickLists();
        }

        private void DeletePokuta()
        {
            if (!EnsureDb()) return;
            if (SelectedPokuta == null) return;

            var entity = _db!.Pokuty.First(p => p.ID_Pokuta == SelectedPokuta.ID_Pokuta);

            _db.Pokuty.Remove(entity);
            _db.SaveChanges();

            Pokuty.Remove(SelectedPokuta);
            SelectedPokuta = null;
            EditPokuta = new Pokuty { DatumPokuty = DateTime.Now };
            PickPronajem = null;

            RefreshPickLists();
            UpdatePokutaCommand.RaiseCanExecuteChanged();
            DeletePokutaCommand.RaiseCanExecuteChanged();
        }
    }
}
