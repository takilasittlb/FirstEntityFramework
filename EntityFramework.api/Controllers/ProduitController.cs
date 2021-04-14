using EntityFramework.api.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EntityFramework.api.Controllers
{
    public class ProduitController : ApiController
    {
        private Entities.DbEntityEntities2 db = new DbEntityEntities2();


        [HttpPost]
        public IHttpActionResult AjouterProduits([FromBody]Produit produits)
        {
            try
            {
                
                var p = db.Produits.FirstOrDefault(x => x.Reference.ToLower() == produits.Reference.ToLower());
                if(p!=null)
                    return Content(HttpStatusCode.Conflict, "cette reference existe deja");

                produits.Id = 0;
                produits.DateCreation = DateTime.Now;
                db.Produits.Add(produits);
                db.SaveChanges();
                return Ok(produits);
            }
            /*
            catch (DbUpdateException ex)
            {
                var exception = ex.InnerException?.InnerException as SqlException;
                if (exception.Number == 2627)
                    return Content(HttpStatusCode.Conflict, "cette reference existe deja");
                return BadRequest(exception?.Message);
            }*/
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult ListeProduit(int index = 0, int taille = 10)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var produits = db.Produits.Include(nameof(Categorie)).OrderByDescending(x => x.DateCreation).
                    Skip(index * taille + 1).Take(taille).ToList();
                return Ok(produits);
            }
            catch (DbUpdateException ex)
            {
                var exception = ex.InnerException?.InnerException as SqlException;
                return BadRequest(exception?.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Supprimer et recuperer un produit


        [HttpDelete]

        public IHttpActionResult SupprimerProduit(int id)
        {

            try
            {
                var produit = db.Produits.Find(id);
                if (produit == null)
                    return Content(HttpStatusCode.NotFound, "Le produit" + id + " n'existe pas");
                db.Produits.Remove(produit);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public IHttpActionResult DetailsProduit(int id)
        {

            try
            {
                var produit = db.Produits.SingleOrDefault(x => x.Id == id);
                if (produit != null)
                    return NotFound();
                return Ok(produit);
            }

            catch (DbUpdateException ex)
            {
                var exception = ex.InnerException?.InnerException as SqlException;

                return BadRequest(exception?.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut]
        public IHttpActionResult ModifierProduit([FromBody] Produit newProduit)
        {

            try
            {
                var oldProduit = db.Produits.AsNoTracking().FirstOrDefault(x => x.Id == newProduit.Id);
                if (oldProduit == null)
                    return Content(HttpStatusCode.NotFound, "le produit "+ newProduit.Id + "n'existe pas");
                newProduit.DateCreation = oldProduit.DateCreation;
                db.Entry(newProduit).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(oldProduit);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
