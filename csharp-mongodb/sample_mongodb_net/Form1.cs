using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace sample_mongodb_net
{
    public enum Accion
    {
        Guardar = 1,
        Actualizar = 2
    }
    public partial class Form1 : Form
    {
        MongoServer Server;
        MongoDatabase Database;

        BsonObjectId IdContacto;
        private Accion _AccionFormulario = Accion.Guardar;
        private Accion AccionFormulario
        {
            get
            {
                return _AccionFormulario;
            }
            set
            {
                if (value == Accion.Guardar)
                    btnGuardar.Text = "&Guardar";
                else
                    btnGuardar.Text = "&Actualizar";
                _AccionFormulario = value;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Server = new MongoServer(new MongoServerSettings()
            {
                Server = new MongoServerAddress("localhost"),
                ConnectionMode = ConnectionMode.Direct,
                IPv6 = false
            });
            Database = Server.GetDatabase("contacts");
            LlenarGrilla();
        }

        private void LlenarGrilla()
        {
            using (Server.RequestStart(Database))
            {
                MongoCollection Contactos = Database.GetCollection<Contact>("contacts");
                dgvEmpleados.DataSource = Contactos.FindAllAs<Contact>().ToList<Contact>();
            }
        }

        private void LimpiarControles()
        {
            txtApellidos.Clear();
            txtNombres.Clear();
            txtTelefono.Clear();
            txtCorreoElectronico.Clear();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarControles();
            AccionFormulario = Accion.Guardar;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (Server.RequestStart(Database))
            {
                MongoCollection Contactos = Database.GetCollection<Contact>("contacts");
                if (AccionFormulario == Accion.Guardar)
                    Contactos.Insert(new Contact()
                    {
                        Apellidos = txtApellidos.Text,
                        Nombres = txtNombres.Text,
                        Telefono = txtTelefono.Text,
                        CorreoElectronico = txtCorreoElectronico.Text
                    });
                else
                {
                    Contact item = Contactos.FindOneByIdAs<Contact>(IdContacto);
                    item.Apellidos = txtApellidos.Text;
                    item.Nombres = txtNombres.Text;
                    item.Telefono = txtTelefono.Text;
                    item.CorreoElectronico = txtCorreoElectronico.Text;
                    Contactos.Save(item, SafeMode.True);
                    AccionFormulario = Accion.Guardar;
                }
            }
            LlenarGrilla();
            LimpiarControles();
        }

        private void tsmiEliminar_Click(object sender, EventArgs e)
        {
            BsonObjectId Id = (BsonObjectId)dgvEmpleados.CurrentRow.Cells["Id"].Value;
            using (Server.RequestStart(Database))
            {
                MongoCollection Contactos = Database.GetCollection<Contact>("contacts");
                Contactos.Remove(Query.EQ("_id", Id));
            }
            LlenarGrilla();
        }

        private void tsmiActualizar_Click(object sender, EventArgs e)
        {
            BsonObjectId Id = (BsonObjectId)dgvEmpleados.CurrentRow.Cells["Id"].Value;
            using (Server.RequestStart(Database))
            {
                MongoCollection Contactos = Database.GetCollection<Contact>("contacts");
                Contact item = Contactos.FindOneByIdAs<Contact>(Id);
                IdContacto = item.Id;
                txtApellidos.Text = item.Apellidos;
                txtNombres.Text = item.Nombres;
                txtTelefono.Text = item.Telefono;
                txtCorreoElectronico.Text = item.CorreoElectronico;
                AccionFormulario = Accion.Actualizar;
            }
        }
    }
}
