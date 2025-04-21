using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO;
using Core.ServicesContracts;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Externals
{
    public class FCMNotificationService : IPushNotificationService
    {
        private readonly IDeviceTokensRepo _deviceTokenRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FirebaseMessaging _fbMessaging;

        public FCMNotificationService(
            IDeviceTokensRepo deviceTokenRepo,
            UserManager<ApplicationUser> userManager
            )
        {
            _deviceTokenRepo = deviceTokenRepo;
            _userManager = userManager;
            _fbMessaging = FirebaseMessaging.DefaultInstance;
        }

        private List<List<string>> Batch(int batchSize, List<string> tokens)
        {
            List<List<string>> batches = [];
            List<string> currentBatch = [];

            for (int i = 0; i < tokens.Count; i++)
            {
                if (currentBatch.Count < batchSize) currentBatch.Add(tokens[i]);
                else
                {
                    List<string> copy = [.. currentBatch];
                    batches.Add(copy);
                    currentBatch.Clear();
                    currentBatch.Add(tokens[i]);
                }
            }

            if (currentBatch.Count != 0) batches.Add([.. currentBatch]);

            return batches;
        }

        public async Task RegisterDevice(int userId, string deviceToken)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new Exception("User does not exist");

            DeviceToken newDeviceToken = new()
            {
                Value = deviceToken,
                UserId = userId
            };

            await _deviceTokenRepo.AddOne(newDeviceToken);
        }

        public async Task UnregisterDevice(string deviceToken)
        {
            DeviceToken? target = await _deviceTokenRepo.GetOneByToken(deviceToken);
            if (target == null) throw new Exception("Device does not exist");
            await _deviceTokenRepo.RemoveOne(target);
        }

        public async Task PushNotificationForOne(string deviceToken, string title, string body)
        {
            DeviceToken? target = await _deviceTokenRepo.GetOneByToken(deviceToken);
            if (target == null) throw new Exception("Device does not exist");

            Message msg = new()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
            };

            try
            {
                await _fbMessaging.SendAsync(msg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BatchResponseDto> PushNotificationForMultiple(List<string> deviceTokens, string title, string body)
        {
            int batchSize = 500;

            List<List<string>> batches = Batch(batchSize, deviceTokens);

            BatchResponseDto result = new();

            foreach (var batch in batches)
            {
                MulticastMessage msg = new()
                {
                    Tokens = batch,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    },
                };

                try
                {
                    var res = await _fbMessaging.SendEachForMulticastAsync(msg);
                    for (int i = 0; i < res.Responses.Count; i++)
                    {
                        var r = res.Responses[i];
                        if (r.IsSuccess) result.SuccessfulDevices.Add(batch[i]);
                        else result.FailedDevices.Add(batch[i]);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }
        public async Task<BatchResponseDto> PushNotificationForAll(string title, string body)
        {
            int batchSize = 500;

            List<string> deviceTokens = (await _deviceTokenRepo.GetAll()).Select(d => d.Value).ToList();

            List<List<string>> batches = Batch(batchSize, deviceTokens);

            BatchResponseDto result = new();

            foreach (var batch in batches)
            {
                MulticastMessage msg = new()
                {
                    Tokens = batch,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    },
                };

                try
                {
                    var res = await _fbMessaging.SendEachForMulticastAsync(msg);
                    for (int i = 0; i < res.Responses.Count; i++)
                    {
                        var r = res.Responses[i];
                        if (r.IsSuccess) result.SuccessfulDevices.Add(batch[i]);
                        else result.FailedDevices.Add(batch[i]);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }
    }
}
