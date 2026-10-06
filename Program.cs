using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using PerpustakaanApp.Models;
using PerpustakaanApp.Database;

// Terdiri dari 2 atau lebih namespace
namespace PerpustakaanApp
{
    // Didokumentasikan dengan baik (XML Comments standard C#)
    /// <summary>
    /// Kelas utama untuk menjalankan aplikasi Sistem Manajemen Perpustakaan.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Menerapkan coding guidelines
            DatabaseHelper.InitializeDatabase();
            
            bool isRunning = true;

            // Pengulangan
            while (isRunning)
            {
                Console.WriteLine("\n=== Sistem Manajemen Perpustakaan ===");
                Console.WriteLine("1. Tambah Buku");
                Console.WriteLine("2. Tampilkan Buku (dari Database)");
                Console.WriteLine("3. Simpan Data ke Media Penyimpan (File JSON)");
                Console.WriteLine("4. Keluar");
                Console.Write("Pilih menu: ");
                
                string pilihan = Console.ReadLine();

                // Percabangan
                if (pilihan == "1")
                {
                    TambahBuku();
                }
                else if (pilihan == "2")
                {
                    TampilkanBuku();
                }
                else if (pilihan == "3")
                {
                    SimpanDataKeFile();
                }
                else if (pilihan == "4")
                {
                    isRunning = false;
                    Console.WriteLine("Terima kasih telah menggunakan sistem ini.");
                }
                else
                {
                    Console.WriteLine("Pilihan tidak valid!");
                }
            }
        }

        // Penggunaan prosedur/fungsi/method
        /// <summary>
        /// Method untuk menambah buku baru ke database.
        /// </summary>
        static void TambahBuku()
        {
            Console.Write("Masukkan ID Buku: ");
            // Error handling tipe data
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID harus berupa angka!");
                return;
            }
            
            Console.Write("Masukkan Judul Buku: ");
            string judul = Console.ReadLine();
            
            Console.Write("Masukkan Penulis Buku: ");
            string penulis = Console.ReadLine();

            Buku bukuBaru = new Buku(id, judul, penulis);
            
            // Simpan ke DB
            DatabaseHelper.InsertBuku(bukuBaru);
            Console.WriteLine("Buku berhasil ditambahkan ke database!");
        }

        /// <summary>
        /// Method untuk menampilkan daftar buku dari database menggunakan array.
        /// </summary>
        static void TampilkanBuku()
        {
            // Penggunaan Array
            Buku[] daftarBuku = DatabaseHelper.GetAllBuku();
            
            if (daftarBuku.Length == 0)
            {
                Console.WriteLine("Tidak ada buku di database.");
                return;
            }

            Console.WriteLine("\nDaftar Buku:");
            // Pengulangan foreach
            foreach (var buku in daftarBuku)
            {
                // Polymorphism (TampilkanInfo)
                buku.TampilkanInfo();
            }
        }

        /// <summary>
        /// Method untuk menyimpan data array buku ke media penyimpan (File JSON).
        /// </summary>
        static void SimpanDataKeFile()
        {
            Buku[] daftarBuku = DatabaseHelper.GetAllBuku();
            
            // Memanfaatkan eksternal library (Newtonsoft.Json)
            string json = JsonConvert.SerializeObject(daftarBuku, Formatting.Indented);
            
            // Menyimpan dan membaca data di media penyimpan
            File.WriteAllText("data_buku.json", json);
            Console.WriteLine("Data berhasil disimpan ke file data_buku.json");
            
            // Simulasi membaca data dari file
            string readJson = File.ReadAllText("data_buku.json");
            Console.WriteLine("Berhasil membaca ulang dari file. Panjang karakter: " + readJson.Length);
        }
    }
}

namespace PerpustakaanApp.Models
{
    // Interface
    /// <summary>
    /// Antarmuka untuk item perpustakaan.
    /// </summary>
    public interface IItemPerpustakaan
    {
        void TampilkanInfo();
    }

    // Inheritance (Base Class)
    /// <summary>
    /// Kelas abstrak dasar untuk item perpustakaan.
    /// </summary>
    public abstract class ItemPerpustakaan : IItemPerpustakaan
    {
        // Hak akses tipe data, Properties
        public int Id { get; protected set; }
        public string Judul { get; protected set; }

        public ItemPerpustakaan(int id, string judul)
        {
            Id = id;
            Judul = judul;
        }

        // Metode abstrak yang harus diimplementasikan oleh kelas turunan
        public abstract void TampilkanInfo();
    }

    // Inheritance (Derived Class)
    /// <summary>
    /// Kelas Buku turunan dari ItemPerpustakaan.
    /// </summary>
    public class Buku : ItemPerpustakaan
    {
        public string Penulis { get; private set; }

        public Buku(int id, string judul, string penulis) : base(id, judul)
        {
            Penulis = penulis;
        }

        // Polymorphism (Override method)
        public override void TampilkanInfo()
        {
            Console.WriteLine($"[Buku] ID: {Id}, Judul: {Judul}, Penulis: {Penulis}");
        }

        // Overloading (Method dengan nama sama tapi parameter beda)
        public void UpdateInfo(string judulBaru)
        {
            Judul = judulBaru;
        }

        public void UpdateInfo(string judulBaru, string penulisBaru)
        {
            Judul = judulBaru;
            Penulis = penulisBaru;
        }
    }
}

namespace PerpustakaanApp.Database
{
    using PerpustakaanApp.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Kelas untuk mengelola operasi basis data (SQLite).
    /// </summary>
    public static class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=perpustakaan.db";

        // Menggunakan basis data
        public static void InitializeDatabase()
        {
            // Memanfaatkan eksternal library (Microsoft.Data.Sqlite)
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Buku (
                        Id INTEGER PRIMARY KEY,
                        Judul TEXT NOT NULL,
                        Penulis TEXT NOT NULL
                    );
                ";
                command.ExecuteNonQuery();
            }
        }

        public static void InsertBuku(Buku buku)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR REPLACE INTO Buku (Id, Judul, Penulis)
                    VALUES ($id, $judul, $penulis)
                ";
                command.Parameters.AddWithValue("$id", buku.Id);
                command.Parameters.AddWithValue("$judul", buku.Judul);
                command.Parameters.AddWithValue("$penulis", buku.Penulis);
                command.ExecuteNonQuery();
            }
        }

        public static Buku[] GetAllBuku()
        {
            List<Buku> listBuku = new List<Buku>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Judul, Penulis FROM Buku";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string judul = reader.GetString(1);
                        string penulis = reader.GetString(2);
                        listBuku.Add(new Buku(id, judul, penulis));
                    }
                }
            }

            // Penggunaan Array (Konversi list ke array)
            return listBuku.ToArray();
        }
    }
}
