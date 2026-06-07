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

**Çağrı Karşılama :** Dışarıdan gelen arama Asterisk santral sistemi tarafından karşılanır. Dialplan (extensions.conf) üzerinde tanımlı Answer() komutuyla hat otomatik olarak açılır ve sistem arka planda kullanıcının sesini anlık olarak kaydetmeye başlar. 

**API Çağrılması :** Kullanıcının konuşması bittiğinde, oluşan ses dosyası sesten metne (STT) dönüştürülür. Asterisk tarafındaki aracı entegrasyon betikleri (AGI veya ARI katmanı), elde edilen bu düz metni alarak bizim geliştirdiğimiz .NET 8 Minimal API endpoint'ine (/api/chat) standart bir HTTP POST isteği gönderir. 

**Ses Oynatma :** Bizim API katmanımızdan dönen yazılı yapay zeka yanıtı, santral tarafında Metinden Sese (TTS) motoruna bir ses dosyasına (.wav) dönüştürülür. Asterisk bu ses dosyasını Playback() veya Background() komutları vasıtasıyla hattaki kullanıcıya dinletir ve akış akıcı bir şekilde devam eder. 

 

 

 

### 🔬 2. STT (Speech To Text)  

Kullanıcının ses verisini metne dönüştürme aşamasında tercihim OpenAI Whisper (özellikle Faster-Whisper implementasyonu) yönündedir. 

Verinin şirket dışına çıkmasını engeller. Bu, özellikle müşteri gizliliği ve KVKK uyumluluğu açısından en kritik tercih sebebidir. 
Telefon hatlarındaki düşük ses kalitesine (8kHz) ve arka plan gürültüsüne rağmen, sunduğu dil modelleri Türkçe dilinde yüksek doğruluk oranı sağlar. 
Faster-Whisper kullanımı ile modelin çıkarım (inference) süresi oldukça kısalır ve GPU hızlandırma desteğiyle çağrı merkezi gibi yüksek trafikli sistemlerde milisaniyeler seviyesinde yanıt verebilir. 
Bulut tabanlı çözümlerin (Google Speech-to-Text veya Azure Speech) her istek başına maliyet çıkarmasının aksine, Whisper açık kaynak kodlu ve ücretsizdir. 

Alternatifler: Bulut mimarisinin zorunlu olduğu veya lokal donanım kısıtının bulunduğu uç durumlarda yüksek performansıyla bilinen Deepgram veya Azure Speech Service değerlendirilebilir. Ancak öncelikli tercihim, donanım optimizasyonu sağlandığı sürece her zaman "lokal ve ücretsiz" olması nedeniyle Whisper'dır. 

 

 

 

### 🔊 3. TTS (Text To Speech) 

API üzerinden dönen metnin seslendirilmesi sürecinde, sistemin hız ve lokal çalışma gereksinimlerine göre Piper TTS veya Coqui TTS modellerini tercih ederim. 

Neden Piper/Coqui TTS : 

Lokal Performans ve Düşük Gecikme: Özellikle Piper TTS, çok düşük kaynak tüketimi ve yüksek çıkarım hızıyla gerçek zamanlı çağrı merkezi senaryoları için  optimize edilmiştir. 

Tamamen Açık Kaynak ve Maliyetsiz: ElevenLabs gibi bulut tabanlı, lisans maliyetleri yüksek olan çözümler yerine, tamamen ücretsiz ve açık kaynaklı modeller kullanarak projenin operasyonel maliyetini (OPEX) sıfıra indirir. 

Veri Güvenliği (Data Sovereignty): Ses sentezleme süreci tamamen kurumun kendi sunucularında gerçekleşir; müşteri verisi veya üretilen ses yanıtları hiçbir dış servise (API dışına) gönderilmez. 

Türkçe Dil Desteği: Güncel açık kaynaklı modeller (özellikle Piper ve Coqui'nin Türkçe ses setleri), telefon hatlarının sınırlı ses frekans aralığında bile oldukça doğal ve anlaşılır bir sentezleme sunmaktadır. 

Alternatifler ve Kıyaslama: 

Kalite Odaklı Yaklaşım: Eğer sistemin önceliği maliyet değil de "insandan ayırt edilemez ses kalitesi" ise, bulut tabanlı ElevenLabs gibi üst düzey çözümler entegre edilebilir. 

Kurumsal Ölçek: Ancak sürdürülebilirlik, KVKK uyumluluğu ve maliyet yönetimi odaklı projelerde, yerel çalışabilen Piper veya Coqui TTS her zaman stratejik tercihimdir. 

 

 

 ###🧠 4. Yapay Zeka (LLM) 

Projede yapay zeka  için tercihim Ollama ekosistemi üzerinde çalışan Llama 3 (veya kaynak yönetimine bağlı olarak Microsoft Phi-3) modelleridir. 

Neden Ollama + Llama 3: 

 

Tam Yerelleşme (On-Premise): Veri güvenliği (KVKK) gereklilikleri nedeniyle, modelin şirket içi donanım üzerinde çalışması bir tercih değil zorunluluktur. Ollama, verinin dışarı çıkmasını %100 engeller. 

Kolay Entegrasyon: Ollama, standart bir REST API arabirimi sunar. Bu, .NET 8 projemizin herhangi bir ek karmaşaya gerek kalmadan HttpClient üzerinden yapay zeka ile doğrudan konuşmasını sağlar. 

Maliyet Etkinliği: OpenAI (GPT) veya Anthropic (Claude) gibi bulut tabanlı API modellerinin aksine, herhangi bir "token başına ücret" maliyeti yoktur. Şirket içi mevcut GPU/CPU kaynaklarını kullanır. 

Ölçeklenebilirlik: İhtiyaca göre (donanım kapasitesine göre) Llama 3 (yüksek performans) veya Phi-3 (düşük gecikme/hafif model) gibi farklı modeller arasında tek bir konfigürasyon değişikliğiyle geçiş yapılabilir. 

Örnek Entegrasyon Akışı: 

.NET API: Kullanıcıdan gelen metni POST http://localhost:11434/api/generate adresine, seçilen model parametreleriyle gönderir. 

Ollama: İsteği alır, yerel donanım üzerinde işler. 

Yanıt: Üretilen metin yanıtı doğrudan .NET katmanına JSON formatında geri döner. 

 ### 🚀 5. Test ve Kurulum Süreci 

Uygulama Kurulumu :

Proje, standart .NET 8 çalışma zamanı üzerinde yapılandırılmıştır. İlk adım olarak ilgili kod deposu yerel ortama kopyalanır (clone). Proje dizininde dotnet restore komutu çalıştırılarak bağımlılıklar tamamlanır ve dotnet run komutu ile uygulama ayağa kaldırılır. API varsayılan olarak http://localhost:5000 adresinde hizmet verir. 

API Test Süreci :

API'nin kararlılığını test etmek için iki yöntem tercih edilir: 

Swagger Arayüzü: http://localhost:5000/swagger adresi üzerinden endpointler interaktif olarak incelenebilir. 

Postman: POST /api/chat adresine {"message": "Merhaba"} JSON içeriği gönderilerek sistemin 200 OK yanıtı verip vermediği doğrulanır. 

Asterisk Entegrasyonu Testi :

Sistemin telefon hattı üzerindeki davranışı şu şekilde doğrulanır: 

Bağlantı: Zoiper veya Linphone gibi bir Softphone uygulaması ile Asterisk sunucusuna bağlanılır. 

Çağrı Başlatma: Softphone üzerinden test numarası aranır. 

Log Takibi: Asterisk CLI konsolu (asterisk -vvvvr) üzerinden çağrının Answer, AGI tetiklenmesi ve Playback adımları canlı olarak gözlemlenir. 

Kullanılan Temel Araçlar 

Backend: .NET 8, Visual Studio 2022 

API Testi: Swagger, Postman 

Telefon Entegrasyonu: Asterisk PBX, Zoiper (Softphone) 

Yapay Zeka & Ses: OpenAI Whisper (STT), Piper (TTS), Ollama (LLM)   
