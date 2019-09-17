using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using datingapp.api.Data;
using datingapp.api.Dtos;
using datingapp.api.Helpers;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace datingapp.api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users/{userid}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository rep;
        private readonly IMapper mapper;
        private readonly IOptions<Cloud> options;
        private readonly Cloudinary c;

        public PhotosController(IDatingRepository rep//chon mikham user ro bgirm va dar nahayat ax zakhire konm
        , IMapper mapper //chon mikham photocreationdto ro map konm b photo asli
        , IOptions<Cloud> options // getting appsetting files using option pattern
        )
        {
            this.rep = rep;
            this.mapper = mapper;
            this.options = options;
            Account account = new Account(
                cloud: options.Value.CloudName,
                apiKey: options.Value.ApiKey,
                apiSecret: options.Value.ApiSecret
            );
            c = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await rep.GetPhoto(id);
            var mappedPhoto = mapper.Map<PhotoForReturnDTO>(photo);
            return Ok(mappedPhoto);
        }





        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int userid, [FromForm] PhotoForCreationDTO photo)
        {
            if (userid != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            } //chon id mostaqiman tu url migirm bayad check konm ba id dakhele jwt token
            var user = await rep.GetUser(userid);
            var file = photo.File;//fili k tu httprequestm bude
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0) // check to see if the file is empty
            {
                using (var stream = file.OpenReadStream() // shoru mikone b khundm file
                )
                {
                    var uploadParams = new ImageUploadParams();
                    uploadParams.File = new FileDescription(file.Name, stream);
                    Transformation t = new Transformation();
                    uploadParams.Transformation = t.Width(500).Height(500).Crop("fill").Gravity("face");
                    uploadResult = c.Upload(uploadParams);
                }
            }

            photo.Url = uploadResult.Uri.ToString();
            photo.PublicId = uploadResult.PublicId;
            var newPhoto = mapper.Map<Photo>(photo);
            if (!user.Photos.Any(e => e.IsMain))
            {
                newPhoto.IsMain = true;
            }
            // newPhoto.UserId = u;
            // newPhoto.User= user;
            user.Photos.Add(newPhoto);

            if (await rep.SaveAll())
            {
                var phototoreturn = mapper.Map<PhotoForReturnDTO>(newPhoto);
                return CreatedAtRoute("GetPhoto", new { id = newPhoto.Id }, phototoreturn);
            }
            return BadRequest("can't save the photo");

        }


        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int id, int userid)
        {
            //throw new Exception("Asdf");
            //check to see if userid and jwt id are equal.
            var claimid = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            if (userid != claimid)
            {
                return Unauthorized();
            }
            var user = await rep.GetUser(userid);
            // check to see if the photo exists
            if (!user.Photos.Any(e => e.Id == id))
            {
                return Unauthorized();
            }
            // check to see if the photo is already the main photo
            var photo = user.Photos.FirstOrDefault(e => e.Id == id);
            if (photo.IsMain)
            {
                return BadRequest("this photo is already the main photo!");
            }
            var mainPhoto = rep.FindMainPhoto(user);
            if(mainPhoto != null) {
                mainPhoto.IsMain = false;
            }
            photo.IsMain = true;

            if (await rep.SaveAll())
            {
                return NoContent();
            }
            throw new Exception("saving changes to photo is not possible!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id, int userid)
        {
            int claimid = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            if (userid != claimid)
            {
                return Unauthorized();
            }
            var user = await rep.GetUser(userid);
            var photo = user.Photos.FirstOrDefault(e => e.Id == id);
            // check to see if the photo exists
            if (photo == null)
            {
                return Unauthorized();
            }
            var publicid = photo.PublicId;
            if (publicid == null) // yani tu cloudinary save nis
            {
                rep.Delete(photo);
            }
            else
            {
                //documentation cloudinary tozih dade k chejuri bayad hazf konm
                DeletionParams p = new DeletionParams(publicid);
                var result = c.Destroy(p);
                if (result.Result == "ok")
                {
                    rep.Delete(photo);
                }
            }

            if (await rep.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("can not delete photo");
        }
    }
}