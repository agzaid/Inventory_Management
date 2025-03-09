using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementation
{
    public class DeliverySlotService : IDeliverySlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeliverySlotService> _logger;  // Inject ILogger<CategoryService>

        public DeliverySlotService(IUnitOfWork unitOfWork, ILogger<DeliverySlotService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public IEnumerable<DeliverySlotVM> GetAllDeliverySlot()
        {
            try
            {
                var deliverySlots = _unitOfWork.DeliverySlot.GetAll(s => s.IsDeleted == false);
                var showDeliverySlots = deliverySlots.Select(s => new DeliverySlotVM()
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    AM_PM = s.AM_PM,
                    IsAvailable = s.IsAvailable,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();

                _logger.LogInformation("GetAllDeliverySlots method completed. {showDeliverySlotsCount} deliverySlots retrieved.", showDeliverySlots.Count);

                return showDeliverySlots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving DeliverySlots.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<string> CreateDeliverySlot(DeliverySlotVM obj)
        {
            try
            {
                var lookForName = _unitOfWork.DeliverySlot.Get(s => s.StartTime == obj.StartTime);
                if (lookForName == null)
                {
                    var delivery = new DeliverySlot()
                    {
                        StartTime = obj.StartTime,
                        EndTime = obj.EndTime,
                        AM_PM = obj.AM_PM,
                        IsAvailable = obj.IsAvailable,
                        Create_Date= DateTime.Now,
                        Modified_Date = DateTime.Now,
                    };
                    _unitOfWork.DeliverySlot.Add(delivery);
                    _unitOfWork.Save();
                    return "Delivery Slot Created Successfully";
                }
                else
                    return "Delivery Slot Already Exists";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating delivery Slot");
                return "Error Occured...";  // Rethrow the exception after logging it
            }
        }

        public DeliverySlotVM GetDeliveryById(int id)
        {
            try
            {
                var deliverySlot = _unitOfWork.DeliverySlot.Get(u => u.Id == id);
                if (deliverySlot != null)
                {
                    var deliverySlotVM = new DeliverySlotVM()
                    {
                        Id = deliverySlot.Id,
                        StartTime = deliverySlot.StartTime,
                        EndTime = deliverySlot.EndTime,
                        AM_PM = deliverySlot.AM_PM,
                        IsAvailable = deliverySlot.IsAvailable,
                        CreatedDate = deliverySlot.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    return deliverySlotVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving deliverySlot with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new DeliverySlotVM();
        }

        public async Task<bool> UpdateDeliverySlot(DeliverySlotVM obj)
        {
            try
            {
                var oldDeliverySlot = _unitOfWork.DeliverySlot.Get(s => s.Id == obj.Id);
                if (oldDeliverySlot != null)
                {
                    oldDeliverySlot.StartTime = obj.StartTime;
                    oldDeliverySlot.EndTime = obj.EndTime;
                    oldDeliverySlot.AM_PM = obj.AM_PM;
                    oldDeliverySlot.IsAvailable = obj.IsAvailable;
                    oldDeliverySlot.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.DeliverySlot.Update(oldDeliverySlot);
                    _unitOfWork.Save();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating category with Id: {Id}", obj.Id);
                return false;  // Rethrow the exception after logging it
            }
        }

        public async Task<bool> DeleteDeliverySlot(int id)
        {
            try
            {
                var oldDelivery = _unitOfWork.DeliverySlot.Get(s => s.Id == id);
                if (oldDelivery != null)
                {
                    oldDelivery.IsDeleted = true;
                    oldDelivery.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.DeliverySlot.Update(oldDelivery);
                    _unitOfWork.Save();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting delivery with Id: {Id}", id);
                return false; // Rethrow the exception after logging it
            }
        }
    }
}
