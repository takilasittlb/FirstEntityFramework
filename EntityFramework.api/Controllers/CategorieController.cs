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
    public class CategorieController : ApiController
    {
        private Entities.DbEntityEntities2 db = new DbEntityEntities2();


        [HttpPost]
        public IHttpActionResult AjouterCategories([FromBody]Categorie categories)
        {
            try
            {
                
                var p = db.Categories.FirstOrDefault(x => x.Nom.ToLower() == categories.Nom.ToLower());
                if(p!=null)
                    return Content(HttpStatusCode.Conflict, "cette reference existe deja");

                categories.Id = 0;
                db.Categories.Add(categories);
                db.SaveChanges();
                return Ok(categories);
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
        public IHttpActionResult ListeCategorie(int index = 0, int taille = 10)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var categories = db.Categories.Include(nameof(Categorie)).OrderByDescending(x => x.Nom).
                    Skip(index * taille + 1).Take(taille).ToList();
                return Ok(categories);
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

        //Supprimer et recuperer un categorie


        [HttpDelete]

        public IHttpActionResult SupprimerCategorie(int id)
        {

            try
            {
                var categorie = db.Categories.Find(id);
                if (categorie == null)
                    return Content(HttpStatusCode.NotFound, "Le categorie" + id + " n'existe pas");
                db.Categories.Remove(categorie);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public IHttpActionResult DetailsCategorie(int id)
        {

            try
            {
                var categorie = db.Categories.SingleOrDefault(x => x.Id == id);
                if (categorie != null)
                    return NotFound();
                return Ok(categorie);
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
        public IHttpActionResult ModifierCategorie([FromBody] Categorie newCategorie)
        {

            try
            {
                var oldCategorie = db.Categories.AsNoTracking().FirstOrDefault(x => x.Id == newCategorie.Id);
                if (oldCategorie == null)
                    return Content(HttpStatusCode.NotFound, "le categorie "+ newCategorie.Id + "n'existe pas");
                db.Entry(newCategorie).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(oldCategorie);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
