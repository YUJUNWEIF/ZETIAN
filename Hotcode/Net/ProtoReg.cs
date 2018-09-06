using System;
using System.Collections.Generic;
using Net;

namespace geniusbaby.pps
{
    public class ProtoReg
    {
        public static void Register()
        {
            try 
            {
                ProtoManager.manager.Register(PID.LockStep, typeof(Net.LockStep));
                ProtoManager.manager.Register(PID.LockStepReconnect, typeof(Net.LockStepReconnect));

            	ProtoManager.manager.Register(PID.AccountRegisterRequest, typeof(AccountRegisterRequest));
            	ProtoManager.manager.Register(PID.AccountRegisterResponse, typeof(AccountRegisterResponse));
            	ProtoManager.manager.Register(PID.AccountLoginRequest, typeof(AccountLoginRequest));
            	ProtoManager.manager.Register(PID.AccountLoginResponse, typeof(AccountLoginResponse));
            	ProtoManager.manager.Register(PID.PlayerCreateRequest, typeof(PlayerCreateRequest));
            	ProtoManager.manager.Register(PID.PlayerCreateResponse, typeof(PlayerCreateResponse));
            	ProtoManager.manager.Register(PID.TimeNotify, typeof(TimeNotify));
            	ProtoManager.manager.Register(PID.VersionCheckRequest, typeof(VersionCheckRequest));
            	ProtoManager.manager.Register(PID.VersionCheckResponse, typeof(VersionCheckResponse));
            	ProtoManager.manager.Register(PID.PlayerNotify, typeof(PlayerNotify));
            	ProtoManager.manager.Register(PID.TokenNotify, typeof(TokenNotify));
            	ProtoManager.manager.Register(PID.ArchiveNotify, typeof(ArchiveNotify));
            	ProtoManager.manager.Register(PID.ArchiveRequest, typeof(ArchiveRequest));
            	ProtoManager.manager.Register(PID.ArchiveResponse, typeof(ArchiveResponse));
            	ProtoManager.manager.Register(PID.GuideNotify, typeof(GuideNotify));
            	ProtoManager.manager.Register(PID.GuideFinishRequest, typeof(GuideFinishRequest));
            	ProtoManager.manager.Register(PID.GuideFinishResponse, typeof(GuideFinishResponse));
            	ProtoManager.manager.Register(PID.KnowledgeNotify, typeof(KnowledgeNotify));
            	ProtoManager.manager.Register(PID.KnowledgeSwitchRequest, typeof(KnowledgeSwitchRequest));
            	ProtoManager.manager.Register(PID.KnowledgeSwitchResponse, typeof(KnowledgeSwitchResponse));
            	ProtoManager.manager.Register(PID.ClassFinishRequest, typeof(ClassFinishRequest));
            	ProtoManager.manager.Register(PID.ClassFinishResponse, typeof(ClassFinishResponse));
            	ProtoManager.manager.Register(PID.WordQueryRequest, typeof(WordQueryRequest));
            	ProtoManager.manager.Register(PID.WordQueryResponse, typeof(WordQueryResponse));
            	ProtoManager.manager.Register(PID.PetNotify, typeof(PetNotify));
            	ProtoManager.manager.Register(PID.PetRmvNotify, typeof(PetRmvNotify));
            	ProtoManager.manager.Register(PID.PetEquipRequest, typeof(PetEquipRequest));
            	ProtoManager.manager.Register(PID.PetEquipResponse, typeof(PetEquipResponse));
            	ProtoManager.manager.Register(PID.PetEggHatchRequest, typeof(PetEggHatchRequest));
            	ProtoManager.manager.Register(PID.PetEggHatchResponse, typeof(PetEggHatchResponse));
            	ProtoManager.manager.Register(PID.PetMergeRequest, typeof(PetMergeRequest));
            	ProtoManager.manager.Register(PID.PetMergeResponse, typeof(PetMergeResponse));
            	ProtoManager.manager.Register(PID.PetLvUpRequest, typeof(PetLvUpRequest));
            	ProtoManager.manager.Register(PID.PetLvUpResponse, typeof(PetLvUpResponse));
            	ProtoManager.manager.Register(PID.PetExplorerSlotNotify, typeof(PetExplorerSlotNotify));
            	ProtoManager.manager.Register(PID.PetExplorerRequest, typeof(PetExplorerRequest));
            	ProtoManager.manager.Register(PID.PetExplorerResponse, typeof(PetExplorerResponse));
            	ProtoManager.manager.Register(PID.PetExplorerGetRequest, typeof(PetExplorerGetRequest));
            	ProtoManager.manager.Register(PID.PetExplorerGetResponse, typeof(PetExplorerGetResponse));
            	ProtoManager.manager.Register(PID.PetExplorerSlotUnlockRequest, typeof(PetExplorerSlotUnlockRequest));
            	ProtoManager.manager.Register(PID.PetExplorerSlotUnlockResponse, typeof(PetExplorerSlotUnlockResponse));
            	ProtoManager.manager.Register(PID.PetExplorerSlotUseRequest, typeof(PetExplorerSlotUseRequest));
            	ProtoManager.manager.Register(PID.PetExplorerSlotUseResponse, typeof(PetExplorerSlotUseResponse));
            	ProtoManager.manager.Register(PID.ItemNotify, typeof(ItemNotify));
            	ProtoManager.manager.Register(PID.ItemRmvNotify, typeof(ItemRmvNotify));
            	ProtoManager.manager.Register(PID.ItemSellRequest, typeof(ItemSellRequest));
            	ProtoManager.manager.Register(PID.ItemSellResponse, typeof(ItemSellResponse));
            	ProtoManager.manager.Register(PID.ItemUseRequest, typeof(ItemUseRequest));
            	ProtoManager.manager.Register(PID.ItemUseResponse, typeof(ItemUseResponse));
            	ProtoManager.manager.Register(PID.ItemSortRequest, typeof(ItemSortRequest));
            	ProtoManager.manager.Register(PID.ItemSortResponse, typeof(ItemSortResponse));
            	ProtoManager.manager.Register(PID.TitleNotify, typeof(TitleNotify));
            	ProtoManager.manager.Register(PID.TitleEquipRequest, typeof(TitleEquipRequest));
            	ProtoManager.manager.Register(PID.TitleEquipResponse, typeof(TitleEquipResponse));
            	ProtoManager.manager.Register(PID.TitleUnlockRequest, typeof(TitleUnlockRequest));
            	ProtoManager.manager.Register(PID.TitleUnlockResponse, typeof(TitleUnlockResponse));
            	ProtoManager.manager.Register(PID.TitleLvUpRequest, typeof(TitleLvUpRequest));
            	ProtoManager.manager.Register(PID.TitleLvUpResponse, typeof(TitleLvUpResponse));
            	ProtoManager.manager.Register(PID.ChatNotify, typeof(ChatNotify));
            	ProtoManager.manager.Register(PID.ChatSendRequest, typeof(ChatSendRequest));
            	ProtoManager.manager.Register(PID.ChatSendResponse, typeof(ChatSendResponse));
            	ProtoManager.manager.Register(PID.GMapChatConnectReport, typeof(GMapChatConnectReport));
            	ProtoManager.manager.Register(PID.GMapChatDisconnReport, typeof(GMapChatDisconnReport));
            	ProtoManager.manager.Register(PID.GMapChatStatusNotify, typeof(GMapChatStatusNotify));
            	ProtoManager.manager.Register(PID.GMapChatAudioSendReport, typeof(GMapChatAudioSendReport));
            	ProtoManager.manager.Register(PID.GMapChatAudioSendNotify, typeof(GMapChatAudioSendNotify));
            	ProtoManager.manager.Register(PID.GMapChatTextSendReport, typeof(GMapChatTextSendReport));
            	ProtoManager.manager.Register(PID.GMapChatTextSendNotify, typeof(GMapChatTextSendNotify));
            	ProtoManager.manager.Register(PID.MailNotify, typeof(MailNotify));
            	ProtoManager.manager.Register(PID.MailRemoveNotify, typeof(MailRemoveNotify));
            	ProtoManager.manager.Register(PID.MailSendRequest, typeof(MailSendRequest));
            	ProtoManager.manager.Register(PID.MailSendResponse, typeof(MailSendResponse));
            	ProtoManager.manager.Register(PID.MailOpenRequest, typeof(MailOpenRequest));
            	ProtoManager.manager.Register(PID.MailOpenResponse, typeof(MailOpenResponse));
            	ProtoManager.manager.Register(PID.MailDeleteRequest, typeof(MailDeleteRequest));
            	ProtoManager.manager.Register(PID.MailDeleteResponse, typeof(MailDeleteResponse));
            	ProtoManager.manager.Register(PID.MailGetAttachmentRequest, typeof(MailGetAttachmentRequest));
            	ProtoManager.manager.Register(PID.MailGetAttachmentResponse, typeof(MailGetAttachmentResponse));
            	ProtoManager.manager.Register(PID.FriendNotify, typeof(FriendNotify));
            	ProtoManager.manager.Register(PID.FriendRemoveNotify, typeof(FriendRemoveNotify));
            	ProtoManager.manager.Register(PID.FriendAddRequest, typeof(FriendAddRequest));
            	ProtoManager.manager.Register(PID.FriendAddResponse, typeof(FriendAddResponse));
            	ProtoManager.manager.Register(PID.FriendRemoveRequest, typeof(FriendRemoveRequest));
            	ProtoManager.manager.Register(PID.FriendRemoveResponse, typeof(FriendRemoveResponse));
            	ProtoManager.manager.Register(PID.FriendAcceptRequest, typeof(FriendAcceptRequest));
            	ProtoManager.manager.Register(PID.FriendAcceptResponse, typeof(FriendAcceptResponse));
            	ProtoManager.manager.Register(PID.FriendGiveRequest, typeof(FriendGiveRequest));
            	ProtoManager.manager.Register(PID.FriendGiveResponse, typeof(FriendGiveResponse));
            	ProtoManager.manager.Register(PID.FriendVisitRequest, typeof(FriendVisitRequest));
            	ProtoManager.manager.Register(PID.FriendVisitResponse, typeof(FriendVisitResponse));
            	ProtoManager.manager.Register(PID.FriendRobberyRequest, typeof(FriendRobberyRequest));
            	ProtoManager.manager.Register(PID.FriendRobberyResponse, typeof(FriendRobberyResponse));
            	ProtoManager.manager.Register(PID.FriendChatRequest, typeof(FriendChatRequest));
            	ProtoManager.manager.Register(PID.FriendChatResponse, typeof(FriendChatResponse));
            	ProtoManager.manager.Register(PID.FriendChatNotify, typeof(FriendChatNotify));
            	ProtoManager.manager.Register(PID.ShopNotify, typeof(ShopNotify));
            	ProtoManager.manager.Register(PID.ShopRequest, typeof(ShopRequest));
            	ProtoManager.manager.Register(PID.ShopResponse, typeof(ShopResponse));
            	ProtoManager.manager.Register(PID.ShopBuyRequest, typeof(ShopBuyRequest));
            	ProtoManager.manager.Register(PID.ShopBuyResponse, typeof(ShopBuyResponse));
            	ProtoManager.manager.Register(PID.AchieveNotify, typeof(AchieveNotify));
            	ProtoManager.manager.Register(PID.AchieveGetAwardRequest, typeof(AchieveGetAwardRequest));
            	ProtoManager.manager.Register(PID.AchieveGetAwardResponse, typeof(AchieveGetAwardResponse));
            	ProtoManager.manager.Register(PID.GSrvPvpRequest, typeof(GSrvPvpRequest));
            	ProtoManager.manager.Register(PID.GSrvPvpResponse, typeof(GSrvPvpResponse));
            	ProtoManager.manager.Register(PID.GSrvPvpNotify, typeof(GSrvPvpNotify));
            	ProtoManager.manager.Register(PID.GMapPvpReport, typeof(GMapPvpReport));
            	ProtoManager.manager.Register(PID.GMapPvpReconnectReport, typeof(GMapPvpReconnectReport));
            	ProtoManager.manager.Register(PID.GMapPvpQuitRequest, typeof(GMapPvpQuitRequest));
            	ProtoManager.manager.Register(PID.GMapPvpQuitResponse, typeof(GMapPvpQuitResponse));
            	ProtoManager.manager.Register(PID.GMapPvpNotify, typeof(GMapPvpNotify));
            	ProtoManager.manager.Register(PID.GMapPvpRmvNotify, typeof(GMapPvpRmvNotify));
            	ProtoManager.manager.Register(PID.GMapPvpConfirmNotify, typeof(GMapPvpConfirmNotify));
            	ProtoManager.manager.Register(PID.GMapPvpConfirmReport, typeof(GMapPvpConfirmReport));
            	ProtoManager.manager.Register(PID.GMapPvpNoSessionNotify, typeof(GMapPvpNoSessionNotify));
            	ProtoManager.manager.Register(PID.GPvpResultReport, typeof(GPvpResultReport));
            	ProtoManager.manager.Register(PID.GMapLinkInfoNotify, typeof(GMapLinkInfoNotify));
            	ProtoManager.manager.Register(PID.GMapPingRequest, typeof(GMapPingRequest));
            	ProtoManager.manager.Register(PID.GMapPingRespone, typeof(GMapPingRespone));
            	ProtoManager.manager.Register(PID.GMapResourceLoadNotify, typeof(GMapResourceLoadNotify));
            	ProtoManager.manager.Register(PID.GMapGameStartNotify, typeof(GMapGameStartNotify));
            	ProtoManager.manager.Register(PID.FightMapLoadProgressNotify, typeof(FightMapLoadProgressNotify));
            	ProtoManager.manager.Register(PID.FightMapLoadProgressReport, typeof(FightMapLoadProgressReport));
            	ProtoManager.manager.Register(PID.GSrvGunFireReport, typeof(GSrvGunFireReport));
            	ProtoManager.manager.Register(PID.GSrvPhysicAttackReport, typeof(GSrvPhysicAttackReport));
            	ProtoManager.manager.Register(PID.GSrvSkillCastReport, typeof(GSrvSkillCastReport));
            	ProtoManager.manager.Register(PID.GSrvTrapProcReport, typeof(GSrvTrapProcReport));
            	ProtoManager.manager.Register(PID.GMapQuizReplyReport, typeof(GMapQuizReplyReport));
            	ProtoManager.manager.Register(PID.GMapQuizEndReport, typeof(GMapQuizEndReport));
            } 
            catch (System.Exception e) 
            {
                Util.Logger.Instance.Error(e.Message, e);
            }
        }
    }
}
