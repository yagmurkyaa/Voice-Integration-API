  ## 📸 
#### Bu projede, .NET 8 ve C# kullanılarak minimalist (Minimal API) mimariyle yapılandırılmış, yüksek performanslı ve hafif bir Web API geliştirilmiştir. Belirlenen endpoint üzerinden gelen mesaj isteklerine hızlı ve kararlı bir şekilde sabit yanıt dönecek altyapı kurulmuş ve Swagger entegrasyonu ile test edilebilirliği sağlanmıştır.

<p align="center">
  <img src="https://github.com/user-attachments/assets/0dcdfee0-62da-47ee-b7d7-74c070d40c4a" alt="Swagger Ekranı 1" width="30%" />
  <img src="https://github.com/user-attachments/assets/fd5e70ae-f7c0-432a-bb6a-57085ef4fa0b" alt="Swagger Ekranı 2" width="30%" />
  <img src="https://github.com/user-attachments/assets/f2c6dffb-7ef9-4026-a72a-308a3f4897ef" alt="Swagger Ekranı 3" width="30%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/07c3c5a3-b9ee-493c-8089-6d5cccaa28a2" alt="Swagger Ekranı 4" width="30%" />
  <img src="https://github.com/user-attachments/assets/677ee0de-1dd5-45c5-9153-c1c3b8003f57" alt="Swagger Ekranı 5" width="30%" />
</p>


### 🛠️ 1. Asterisk Entegrasyonu  

Geliştirilen .NET 8 Web API'sinin bir Asterisk telefon santrali ile uçtan uca konuşabilmesi için kurulan entegrasyon mantığı şu şekildedir : 

* **Çağrı Karşılama :** Dışarıdan gelen arama Asterisk santral sistemi tarafından karşılanır. Dialplan (extensions.conf) üzerinde tanımlı Answer() komutuyla hat otomatik olarak açılır ve sistem arka planda kullanıcının sesini anlık olarak kaydetmeye başlar. 

* **API Çağrılması :** Kullanıcının konuşması bittiğinde, oluşan ses dosyası sesten metne (STT) dönüştürülür. Asterisk tarafındaki aracı entegrasyon betikleri (AGI veya ARI katmanı), elde edilen bu düz metni alarak bizim geliştirdiğimiz .NET 8 Minimal API endpoint'ine (/api/chat) standart bir HTTP POST isteği gönderir. 

* **Ses Oynatma :** Bizim API katmanımızdan dönen yazılı yapay zeka yanıtı, santral tarafında Metinden Sese (TTS) motoruna bir ses dosyasına (.wav) dönüştürülür. Asterisk bu ses dosyasını Playback() veya Background() komutları vasıtasıyla hattaki kullanıcıya dinletir ve akış akıcı bir şekilde devam eder. 

 

 

 

### 🔬 2. STT (Speech To Text) 

Kullanıcıdan gelen ses verisini metne dönüştürme aşamasında birincil tercihim **OpenAI Whisper** (özellikle **Faster-Whisper** implementasyonu) yönündedir.

#### 🎯 Neden Faster-Whisper?
* **Veri Güvenliği & KVKK :** Tamamen lokal (on-premise) sunucularda çalıştırılabildiği için müşteri verisi şirket dışına çıkmaz; KVKK uyumluluğu tam sağlanır.
* **Performans & Hız :** Düşük kaliteli telefon hatlarında ve gürültülü ortamlarda Türkçe doğruluk oranı çok yüksektir. GPU hızlandırması ve optimize edilmiş altyapısı sayesinde çıkarım süreleri milisaniyeler seviyesindedir.
* **Sıfır Lisans Maliyeti:** Bulut servislerinin (Google/Azure) aksine açık kaynak kodlu ve tamamen ücretsizdir.
 

### 🔊 3. TTS (Text To Speech) 

API üzerinden dönen yanıt metninin sese dönüştürülmesi sürecinde, hız ve lokal çalışma gereksinimleri doğrultusunda **Piper TTS** veya **Coqui TTS** modellerini tercih ederim.

#### 🎯 Neden Piper / Coqui TTS?
* **Düşük Gecikme & Performans :** Özellikle Piper TTS, minimum kaynak tüketimi ve ultra yüksek çıkarım (inference) hızı sayesinde gerçek zamanlı çağrı senaryoları için mükemmel bir optimizasyon sunar.
* **Sıfır Operasyonel Maliyet (OPEX):** ElevenLabs gibi yüksek lisans maliyeti olan bulut çözümlerinin aksine, tamamen açık kaynaklı ve ücretsizdir.
* **Müşteri Verisi Güvenliği :** Ses sentezleme süreci kurumun kendi lokal sunucularında tamamlanır; ses dosyaları veya müşteri verileri asla dış dünyaya sızmaz.
* **Doğal Türkçe Desteği :** Modellerin güncel Türkçe ses setleri, telefon hatlarının kısıtlı frekans aralığında bile oldukça doğal, akıcı ve anlaşılır bir sentezleme sağlar.

> 💡 **Alternatif Senaryo :** Eğer projenin önceliği maliyet veya veri gizliliği değil de tamamen "insandan ayırt edilemez ses kalitesi" ise, bulut tabanlı **ElevenLabs** mimariye entegre edilebilir. Ancak sürdürülebilirlik ve KVKK odaklı kurumsal projelerde tercihim yerel modellerdir.
 
### 🧠 4. Yapay Zeka 

Projenin zeka katmanında, yapay zeka modellerini yerel bilgisayarda çalıştırmayı sağlayan **Ollama** altyapısı ve akıllı yanıt motoru olarak da **Llama 3** (veya donanım kısıtlarına göre daha hafif olan **Microsoft Phi-3**) modeli tercih ederdim. 

#### 🎯 Neden Ollama + Llama 3?
* **%100 Yerel Çalışma (On-Premise) :** Veri güvenliği ve KVKK regülasyonları gereği model tamamen şirket içi donanımda çalışır ; veri asla dışarı sızmaz.
* **Kolay .NET Entegrasyonu :** Ollama'nın sunduğu standart REST API arabirimi sayesinde, .NET 8 projemiz karmaşık kütüphanelere ihtiyaç duymadan `HttpClient` üzerinden yapay zeka ile doğrudan konuşabilir.
* **Sıfır Token Maliyeti :** OpenAI veya Anthropic gibi bulut servislerinin aksine "token başına" ücretlendirme yoktur ; mevcut donanım kaynakları verimli şekilde kullanılır.

#### 🔄 Örnek Entegrasyon Akışı
1. **.NET API :** Gelen metni `POST http://localhost:11434/api/generate` adresine JSON olarak gönderir.
2. **Ollama :** İsteği yakalar ve yerel GPU/CPU üzerinde modeli koşturarak yanıtı üretir.
3. **Yanıt :** Üretilen metin, .NET katmanına anında teslim edilir.
 

 ### 🚀 5. Test ve Kurulum Süreci

#### 📦 Projenin Çalıştırılması
1. Proje klasöründeki ana dosyayı **Visual Studio** ile açın ve yukarıdaki **"Çalıştır" (Run)** butonuna basın.
2. Uygulama ayağa kalktığında tarayıcınızda otomatik olarak interaktif bir test sayfası (**Swagger arayüzü**) açılacaktır.

#### 🧪 API Nasıl Test Edilir?
* **Açılan Sayfa Üzerinden (Swagger):** `http://localhost:5000/swagger` adresindeki arayüzde `POST /api/chat` endpoint'ine tıklayın. Gelen kutucuğa `{"message": "Merhaba"}` JSON içeriğini yazıp çalıştırdığınızda, yapay zekadan dönen cevabı doğrudan ekranda görebilirsiniz.
* **Postman ile:** Postman uygulamasını açıp `POST /api/chat` adresine `{"message": "Merhaba"}` içeriğini göndererek sistemin `200 OK` yanıtı verip vermediğini doğrulayabilirsiniz.

#### 📞 Asterisk ile Nasıl Test Edilir?
1. **Bağlantı:** Zoiper veya Linphone gibi bir Softphone (yazılımsal telefon) uygulaması ile Asterisk sunucusuna bağlanılır.
2. **Çağrı Başlatma:** Softphone üzerinden sistemde tanımlı test numarası aranarak akış tetiklenir.
3. **Log Takibi:** Asterisk CLI konsolu (`asterisk -vvvvr`) açılarak çağrının karşılanması (`Answer`), ses analiz betiğinin tetiklenmesi (`AGI`) ve yapay zeka yanıtının sesli okunması (`Playback`) adımları canlı loglar üzerinden anlık olarak izlenir.

#### 🛠️ Kullanılan Temel Araçlar
* **Backend:** .NET, Visual Studio 2022
* **API Testi:** Swagger (Tarayıcı Arayüzü), Postman
* **Telefon Entegrasyonu:** Asterisk PBX, Zoiper / Linphone (Softphone)
* **Yapay Zeka & Ses:** OpenAI Whisper (STT), Piper (TTS), Ollama (LLM)

Yapay Zeka & Ses: OpenAI Whisper (STT), Piper (TTS), Ollama (LLM)   
