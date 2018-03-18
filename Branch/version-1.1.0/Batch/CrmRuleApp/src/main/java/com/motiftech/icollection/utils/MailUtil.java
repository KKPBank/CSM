package com.motiftech.icollection.utils;

import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.mail.MessagingException;
import javax.mail.internet.MimeMessage;

import org.apache.commons.lang.StringUtils;
import org.apache.log4j.Logger;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.util.Assert;

import com.icollection.bean.MailBean;
import com.utils.TemplateFileUtil;

public class MailUtil {

	private final Logger log = Logger.getLogger(getClass());
	private Pattern varPattern = Pattern.compile("%(\\w*)%");
	private JavaMailSender mailSender;
	private TemplateFileUtil templateFileUtil;
	private String defaultFrom;

	public boolean sendSimpleMail(String from, String[] to, String subject, String message, boolean isHtml) {
		Assert.notEmpty(to, "at least one recipient must be addressed");
		Assert.notNull(subject, "subject must not be null");
		Assert.notNull(message, "message must not be null");
		if (StringUtils.isBlank(from)) {
			from = this.defaultFrom;
		}
		try {
			MimeMessage mimeMessage = this.mailSender.createMimeMessage();
			MimeMessageHelper messageHelper = new MimeMessageHelper(mimeMessage);

			messageHelper.setFrom(from);
			messageHelper.setTo(to);
			messageHelper.setSubject(subject);
			messageHelper.setText(message, isHtml);

			this.mailSender.send(mimeMessage);
			return true;
		} catch (MessagingException ex) {
			this.log.error("cannot send mail", ex);
		}
		return false;
	}

	public MailBean createMailMessage(String template, Map<String, String> varMap) {
		String filename = getTemplateFile(template);
		List<String> templateData = this.templateFileUtil.readEmailTemplate(filename);
		MailBean mailBean = new MailBean();
		String subject = "";
		StringBuffer message = new StringBuffer();
		if (!templateData.isEmpty()) {
			subject = replaceVariable((String) templateData.get(0), varMap);
			for (int i = 1; i < templateData.size(); i++) {
				message.append(replaceVariable((String) templateData.get(i), varMap));
				message.append("\n");
			}
			mailBean.setSubject(subject);
			mailBean.setMessage(message.toString());
		}
		return mailBean;
	}

	protected String getTemplateFile(String template) {
		return "email-" + template + ".txt";
	}

	protected String replaceVariable(String text, Map<String, String> varMap) {
		StringBuffer sb = new StringBuffer();
		Matcher m = this.varPattern.matcher(text);
		while (m.find()) {
			String key = m.group(m.groupCount());
			String value = StringUtils.defaultString((String) varMap.get(key));

			m.appendReplacement(sb, value);
		}
		m.appendTail(sb);

		return sb.toString();
	}

	public void setMailSender(JavaMailSender mailSender) {
		this.mailSender = mailSender;
	}

	public void setTemplateFileUtil(TemplateFileUtil templateFileUtil) {
		this.templateFileUtil = templateFileUtil;
	}

	public void setVarPattern(String varPattern) {
		this.varPattern = Pattern.compile(varPattern);
	}

	public void setDefaultFrom(String defaultFrom) {
		this.defaultFrom = defaultFrom;
	}
}
